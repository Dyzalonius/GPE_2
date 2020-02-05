using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Vector2Int gridSize;

    [SerializeField]
    [Range(0, 20)]
    private int splitCount;

    [SerializeField]
    [Range(0f, 0.5f)]
    private float minSplitPercentage;

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
        if (Input.GetKeyDown(KeyCode.G))
            GenerateLevel();
    }

    private void GenerateLevel()
    {
        ResetLevel();

        Grid = new TileType[gridSize.x, gridSize.y];
        SetBlock(0, 0, gridSize.x, gridSize.y, TileType.Wall);

        Node root = new Node(0, 0, gridSize.x, gridSize.y);
        root.Split(splitCount, minSplitPercentage);
        root.Draw();

        /*SetBlock(26, 26, 12, 12, TileType.Empty);
        SetBlock(30, 30, 1, 1, TileType.Dagger);
        SetBlock(34, 30, 1, 1, TileType.Key);
        SetBlock(32, 32, 1, 1, TileType.Door);
        SetBlock(32, 36, 1, 1, TileType.Enemy);
        SetBlock(32, 34, 1, 1, TileType.End);*/
        SetBlock(32, 28, 1, 1, TileType.Player);
        //Debugger.instance.AddLabel(32, 26, "Room 1");

        CreateLevel();
    }

    public void SetBlock(int x, int y, int width, int height, TileType fillType)
    {
        for (int tileY = 0; tileY < height; tileY++)
        {
            for (int tileX = 0; tileX < width; tileX++)
            {
                SetTile(x + tileX, y + tileY, fillType);
            }
        }
    }

    private void SetTile(int x, int y, TileType fillType)
    {
        // Check bounds
        if (x < 0) return;
        if (y < 0) return;
        if (x >= Grid.GetLength(1)) return;
        if (y >= Grid.GetLength(0)) return;

        Grid[y, x] = fillType;
    }

    private void CreateLevel() {
        int height = Grid.GetLength(0);
        int width = Grid.GetLength(1);
        for (int y=0; y<height; y++) {
            for (int x=0; x<width; x++) {
                 TileType tile = Grid[y, x];
                 if (tile != TileType.Empty) {
                     CreateTile(x, y, tile);
                 }
            }
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