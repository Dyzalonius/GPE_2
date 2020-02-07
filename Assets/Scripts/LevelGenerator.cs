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

        Grid = new TileType[gridSize.x, gridSize.y];
        SetBlock(Vector2Int.zero, gridSize, TileType.Wall);

        Node root = new Node(0, 0, gridSize.x, gridSize.y);
        root.Split(splitCount, minSplitPercentage);
        root.Draw();
        
        SetBlock(new Vector2Int(32, 28), Vector2Int.one, TileType.Player);

        CreateLevel();
    }

    public void SetBlock(Vector2Int pos, Vector2Int size, TileType fillType)
    {
        for (int i = 0; i < size.y; i++)
            for (int j = 0; j < size.x; j++)
                SetTile(new Vector2Int(pos.x + j, pos.y + i), fillType);
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