using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Node
{
    private int x;
    private int y;
    private int width;
    private int height;
    private Node[] children = new Node[2];
    private bool hasChildren = false;
    
    public Node(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public void Split(int splitCount, float minSplitPercentage = 0.5f)
    {
        // Exit if no more splits left
        if (splitCount <= 0)
            return;
        else
            splitCount--;

        // Calculate splitpercentage and if we are splitting through X axis or Y axis
        float splitPercentage = Random.Range(minSplitPercentage, 1 - minSplitPercentage);
        bool splitThroughX = width == height ? Random.value > 0.5 : width > height;

        // Create new children
        hasChildren = true;
        if (splitThroughX)
        {
            Node child1 = new Node(x, y, Mathf.RoundToInt(width * splitPercentage), height);
            Node child2 = new Node(x + Mathf.RoundToInt(width * splitPercentage), y, Mathf.RoundToInt(width * (1f - splitPercentage)), height);
            children[0] = child1;
            children[1] = child2;
        }
        else
        {
            Node child1 = new Node(x, y, width, Mathf.RoundToInt(height * splitPercentage));
            Node child2 = new Node(x, y + Mathf.RoundToInt(height * splitPercentage), width, Mathf.RoundToInt(height * (1f - splitPercentage)));
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
            LevelGenerator.Instance.SetBlock(x, y, width, height, TileType.Wall);
            LevelGenerator.Instance.SetBlock(x + 1, y + 1, width - 2, height - 2, TileType.Empty);
            Debugger.instance.AddLabel(x + width/2, y + height/2, "ThisIsANode");
        }
    }
}
