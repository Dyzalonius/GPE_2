using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    private Vector2Int pos;
    private Vector2Int size;
    private Node[] children = new Node[2];
    private bool hasChildren = false;
    private Room room;
    
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
        {
            room = new Room(pos, size);
            return;
        }
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

    public List<Room> GetRooms()
    {
        List<Room> rooms = new List<Room>();

        if (hasChildren)
        {
            rooms.AddRange(children[0].GetRooms());
            rooms.AddRange(children[1].GetRooms());
        }
        else
            rooms.Add(room);

        return rooms;
    }

    public void ConnectChildren()
    {
        List<Room> rooms1 = children[0].GetRooms();
        List<Room> rooms2 = children[1].GetRooms();

        Room randomRoom = rooms2[Random.Range(0, rooms2.Count - 1)];
        Room closestRoom1 = rooms1.OrderBy(x => Vector2Int.Distance(x.CenterPos, randomRoom.CenterPos)).ToArray()[0];
        Room closestRoom2 = rooms2.OrderBy(x => Vector2Int.Distance(x.CenterPos, closestRoom1.CenterPos)).ToArray()[0];
        closestRoom1.CreateCorridor(closestRoom2);

        if (children[0].hasChildren)
        {
            children[0].ConnectChildren();
            children[1].ConnectChildren();
        }
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
            room.Draw();
        }
    }
}
