using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField]
    [Range(0, 20)]
    private int splitCount;

    [SerializeField]
    [Range(0f, 0.5f)]
    private float minSplitPercentage; // Known issue: splitPercentage can be lower than the minRoomSize and larger than the maxRoomSize

    [SerializeField]
    private Vector2Int gridSize;

    [Header("Room size settings")]
    [SerializeField]
    public int MinRoomSizeFlat;

    [SerializeField]
    public int MaxRoomSizeFlat;

    [SerializeField]
    [Range(0f, 1f)]
    public float MinRoomSizePercentage;

    [SerializeField]
    [Range(0f, 1f)]
    public float MaxRoomSizePercentage;

    [HideInInspector]
    public static LevelGenerator Instance;

    [HideInInspector]
    public GameObject[] tiles;

    [HideInInspector]
    public TileType[,] Grid;

    [HideInInspector]
    public int roomIndex;

    private void Start()
    {
        Instance = this;
        GenerateLevel();
    }

    private void Update()
    {
        if (MinRoomSizeFlat < 1)
            MinRoomSizeFlat = 1;
        if (MaxRoomSizeFlat < MinRoomSizeFlat)
            MaxRoomSizeFlat = MinRoomSizeFlat;
        if (MaxRoomSizePercentage < MinRoomSizePercentage)
            MaxRoomSizePercentage = MinRoomSizePercentage;

        if (Input.GetKeyDown(KeyCode.G))
            GenerateLevel();
    }

    private void GenerateLevel()
    {
        ResetLevel();

        // Generate
        Grid = new TileType[gridSize.x, gridSize.y];
        Node root = new Node(0, 0, gridSize.x, gridSize.y);
        root.Split(splitCount, minSplitPercentage);
        root.ConnectChildren();
        List<Room> rooms = root.GetRooms();
        List<Room> leafRooms = rooms.FindAll(x => x.ConnectedRooms.Count == 1);
        int leafCount = leafRooms.Count;
        
        // Pick locations
        Room startRoom = leafRooms[Random.Range(0, leafRooms.Count)];
        leafRooms.Remove(startRoom);
        rooms.Remove(startRoom);

        Room endRoom = leafRooms[Random.Range(0, leafRooms.Count)];
        leafRooms.Remove(endRoom);
        rooms.Remove(endRoom);

        Room daggerRoom = rooms[Random.Range(0, rooms.Count)];
        leafRooms.Remove(daggerRoom);
        rooms.Remove(daggerRoom);

        Room keyRoom = rooms[Random.Range(0, rooms.Count)];
        leafRooms.Remove(keyRoom);
        rooms.Remove(keyRoom);

        Room enemyRoom = rooms[Random.Range(0, rooms.Count)];
        leafRooms.Remove(enemyRoom);
        rooms.Remove(enemyRoom);

        // Draw locations
        SetBlock(Vector2Int.zero, gridSize, TileType.Wall);
        root.Draw();
        SetBlock(startRoom.CenterPos, Vector2Int.one, TileType.Player);
        SetBlock(endRoom.CenterPos, Vector2Int.one, TileType.End);
        SetBlock(daggerRoom.CenterPos, Vector2Int.one, TileType.Dagger);
        SetBlock(keyRoom.CenterPos, Vector2Int.one, TileType.Key);
        SetBlock(enemyRoom.CenterPos, Vector2Int.one, TileType.Enemy);
        
        // Wait to make sure the doorpieces are not placed on other tiles
        CreateLevel();

        List<Vector2Int> doorPieces = new List<Vector2Int>();
        for (int i = endRoom.Pos.x - 1; i < endRoom.Pos.x + endRoom.Size.x + 1; i++)
            for (int j = endRoom.Pos.y - 1; j < endRoom.Pos.y + endRoom.Size.y + 1; j++)
            {
                if (i < 0 || j < 0 || i >= Grid.GetLength(1) || j >= Grid.GetLength(0)) continue;
                if ((i == endRoom.Pos.x - 1 || i == endRoom.Pos.x + endRoom.Size.x || j == endRoom.Pos.y - 1 || j == endRoom.Pos.y + endRoom.Size.y))
                    doorPieces.Add(new Vector2Int(i, j));
            }
        doorPieces.ForEach(x => SetBlock(x, Vector2Int.one, TileType.Door));

        CreateLevel();
    }

    public void SetBlock(Vector2Int pos, Vector2Int size, TileType fillType)
    {
        for (int i = 0; i < size.y; i++)
            for (int j = 0; j < size.x; j++)
                SetTile(new Vector2Int(pos.x + j, pos.y + i), fillType);
    }

    public void SetBlockWithCorners(Vector2Int pos1, Vector2Int pos2, TileType fillType)
    {
        Vector2Int bottomLeft = new Vector2Int(Mathf.Min(pos1.x, pos2.x), Mathf.Min(pos1.y, pos2.y));
        Vector2Int topRight = new Vector2Int(Mathf.Max(pos1.x, pos2.x), Mathf.Max(pos1.y, pos2.y));
        SetBlock(bottomLeft, topRight - bottomLeft + Vector2Int.one, fillType);
    }

    private void SetTile(Vector2Int pos, TileType fillType)
    {
        // Check bounds
        if (pos.x < 0) return;
        if (pos.y < 0) return;
        if (pos.x >= Grid.GetLength(1)) return;
        if (pos.y >= Grid.GetLength(0)) return;

        Grid[pos.y, pos.x] = fillType;
    }

    private void CreateLevel() {
        int height = Grid.GetLength(0);
        int width = Grid.GetLength(1);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                 TileType tile = Grid[y, x];
                 if (tile != TileType.Empty)
                     CreateTile(x, y, tile);
            }
    }

    private GameObject CreateTile(int x, int y, TileType type) {
        int tileID = ((int)type) - 1;
        if (tileID >= 0 && tileID < tiles.Length)
        {
            GameObject tilePrefab = tiles[tileID];
            if (tilePrefab != null) {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                newTile.transform.SetParent(transform);
                return newTile;
            }

        } else {
            Debug.LogError("Invalid tile type selected");
        }

        return null;
    }

    private void ResetLevel()
    {
        roomIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        Debugger.instance.labels.ForEach(x => Destroy(x));
        Debugger.instance.labels.Clear();
    }
}

public enum TileType
{
    Empty = 0,
    Player,
    Enemy,
    Wall,
    Door,
    Key,
    Dagger,
    End
}