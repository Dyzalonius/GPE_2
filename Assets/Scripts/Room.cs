using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Room
{
    public Vector2Int CenterPos
    {
        get
        {
            return new Vector2Int(Pos.x + Size.x / 2, Pos.y + Size.y / 2);
        }
    }

    public List<Room> ConnectedRoomsList
    {
        get
        {
            List<Room> rooms = new List<Room>();
            foreach (var pair in ConnectedRooms)
                rooms.Add(pair.Key);

            return rooms;
        }
    }

    public Vector2Int Pos;
    public Vector2Int Size;
    private int index;

    public Dictionary<Room, bool> ConnectedRooms = new Dictionary<Room, bool>(); // bool is for owner of the corridor (only the owner draws the corridor)

    public Room(Vector2Int pos, Vector2Int size)
    {
        Create(pos, size);
        index = LevelGenerator.Instance.roomIndex;
        LevelGenerator.Instance.roomIndex++;
    }

    public void Create(Vector2Int pos, Vector2Int size)
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

        this.Size = new Vector2Int
        {
            x = Mathf.RoundToInt(Random.Range(minRoomSize.x, maxRoomSize.x)),
            y = Mathf.RoundToInt(Random.Range(minRoomSize.y, maxRoomSize.y))
        };
        this.Pos = new Vector2Int
        {
            x = pos.x + Mathf.RoundToInt(Random.Range(1f, size.x - this.Size.x - 1)),
            y = pos.y + Mathf.RoundToInt(Random.Range(1f, size.y - this.Size.y - 1))
        };
    }

    public void CreateCorridor(Room room)
    {
        if (ConnectedRooms.ContainsKey(room)) return;

        ConnectedRooms.Add(room, true);
        room.ConnectedRooms.Add(this, false);
    }

    public void Draw()
    {
        LevelGenerator.Instance.SetBlock(Pos, Size, TileType.Empty);

        string label = "Room " + index + " (";
        int i = 0;
        foreach (var pair in ConnectedRooms)
        {
            label += pair.Key.index;
            if (i < ConnectedRooms.Count - 1)
                label += ", ";
            i++;
        }
        label += ")";
        Debugger.instance.AddLabel(Pos.x + Size.x / 2, Pos.y, label);

        // Draw corridors if owner of corridor
        foreach (var pair in ConnectedRooms)
            if (pair.Value)
                DrawCorridor(pair.Key);
    }

    private void DrawCorridor(Room room)
    {
        Vector2Int middlePos = new Vector2Int(CenterPos.x, room.CenterPos.y);
        LevelGenerator.Instance.SetBlockWithCorners(CenterPos, middlePos, TileType.Empty);
        LevelGenerator.Instance.SetBlockWithCorners(middlePos, room.CenterPos, TileType.Empty);
    }
}
