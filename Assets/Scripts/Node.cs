using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Node
{
    private Vector2Int pos;
    private Vector2Int size;
    private Node[] children = new Node[2];
    private bool hasChildren = false;
    
    public Node(int x, int y, int width, int height)
    {
        pos.x = x;
        pos.y = y;
        size.x = width;
        size.y = height;
    }

    public void Split(int splitCount, float minSplitPercentage = 0.5f)
    {
        // Exit if no more splits left
        if (splitCount <= 0)
            return;
        else
            splitCount--;

        // Decide if we are splitting through X axis or Y axis and calculate splitPercentage
        bool splitThroughX = size.x == size.y ? Random.value > 0.5 : size.x > size.y;
        float splitPercentage = Random.Range(minSplitPercentage, 1 - minSplitPercentage);

        // Create new children
        hasChildren = true;
        if (splitThroughX)
        {
            Node child1 = new Node(pos.x, pos.y, Mathf.RoundToInt(size.x * splitPercentage), size.y);
            Node child2 = new Node(pos.x + Mathf.RoundToInt(size.x * splitPercentage), pos.y, Mathf.RoundToInt(size.x * (1f - splitPercentage)), size.y);
            children[0] = child1;
            children[1] = child2;
        }
        else
        {
            Node child1 = new Node(pos.x, pos.y, size.x, Mathf.RoundToInt(size.y * splitPercentage));
            Node child2 = new Node(pos.x, pos.y + Mathf.RoundToInt(size.y * splitPercentage), size.x, Mathf.RoundToInt(size.y * (1f - splitPercentage)));
            children[0] = child1;
            children[1] = child2;
        }

        // Split recursively for children
        children[0].Split(splitCount, minSplitPercentage);
        children[1].Split(splitCount, minSplitPercentage);
    }

    public void Draw()
    {
        if (hasChildren)
        {
            children[0].Draw();
            children[1].Draw();
        }
        else
        {
            Vector2 minRoomSize = new Vector2
            {
                x = Mathf.Clamp(LevelGenerator.Instance.MinRoomSizePercentage * size.x, LevelGenerator.Instance.MinRoomSizeFlat, size.x - 2f),
                y = Mathf.Clamp(LevelGenerator.Instance.MinRoomSizePercentage * size.y, LevelGenerator.Instance.MinRoomSizeFlat, size.y - 2f)
            };
            Vector2 maxRoomSize = new Vector2
            {
                x = Mathf.Clamp(LevelGenerator.Instance.MaxRoomSizePercentage * size.x, minRoomSize.x, Mathf.Min(LevelGenerator.Instance.MaxRoomSizeFlat, size.x - 2f)),
                y = Mathf.Clamp(LevelGenerator.Instance.MaxRoomSizePercentage * size.y, minRoomSize.y, Mathf.Min(LevelGenerator.Instance.MaxRoomSizeFlat, size.y - 2f))
            };
            Vector2Int roomSize = new Vector2Int
            {
                x = Mathf.RoundToInt(Random.Range(minRoomSize.x, maxRoomSize.x)),
                y = Mathf.RoundToInt(Random.Range(minRoomSize.y, maxRoomSize.y))
            };
            Vector2Int roomPos = new Vector2Int
            {
                x = pos.x + Mathf.RoundToInt(Random.Range(1f, size.x - roomSize.x - 1)),
                y = pos.y + Mathf.RoundToInt(Random.Range(1f, size.y - roomSize.y - 1))
            };

            LevelGenerator.Instance.SetBlock(pos, size, TileType.Wall);
            LevelGenerator.Instance.SetBlock(roomPos, roomSize, TileType.Empty);
            Debugger.instance.AddLabel(pos.x + size.x / 2, pos.y + size.y / 2, "ThisIsANode");
        }
    }
}
