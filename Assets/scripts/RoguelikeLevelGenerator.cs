using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoguelikeLevelGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap ground;
    public Tilemap walls;

    [Header("Tiles (можно 1 или набор вариаций)")]
    public TileBase[] floorTiles;
    public TileBase[] wallTiles;

    [Header("Map Size")]
    public int width = 120;
    public int height = 80;

    [Header("Seed (0 = random)")]
    public int seed = 0;

    [Header("BSP / Rooms")]
    public int minLeafSize = 20;
    public int minRoomSize = 7;
    public int maxRoomPadding = 3;

    [Header("Auto Generate On Start")]
    public bool autoGenerate = true;

    // 0 = wall, 1 = floor
    int[,] map;

    // roomIdMap[x,y] = -1 (не комната: стена/коридор) или 0..rooms-1
    public int[,] roomIdMap { get; private set; }

    public struct Room
    {
        public int id;
        public RectInt rect;
        public Vector2Int center;
        public int Area => rect.width * rect.height;
    }

    public List<Room> rooms { get; private set; } = new List<Room>();

    // Для каждой комнаты: список клеток "дверей" (клетки коридора рядом с комнатой)
    public List<Vector3Int>[] roomDoors { get; private set; }

    System.Random rnd;

    void Start()
    {
        if (autoGenerate)
            Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        InitRandom();
        InitMaps();

        rooms.Clear();

        // BSP root (оставляем рамку стен)
        var root = new Leaf(new RectInt(1, 1, width - 2, height - 2));
        var leaves = new List<Leaf> { root };

        // Split
        bool didSplit = true;
        while (didSplit)
        {
            didSplit = false;
            for (int i = 0; i < leaves.Count; i++)
            {
                var l = leaves[i];
                if (l.left != null || l.right != null) continue;

                if (l.rect.width > minLeafSize || l.rect.height > minLeafSize)
                {
                    if (l.Split(rnd, minLeafSize))
                    {
                        leaves.Add(l.left);
                        leaves.Add(l.right);
                        didSplit = true;
                    }
                }
            }
        }

        // Rooms
        foreach (var l in leaves)
        {
            if (l.left != null || l.right != null) continue;

            RectInt roomRect = CreateRoomInLeaf(l.rect);
            int id = rooms.Count;

            var room = new Room
            {
                id = id,
                rect = roomRect,
                center = new Vector2Int(roomRect.xMin + roomRect.width / 2, roomRect.yMin + roomRect.height / 2)
            };
            rooms.Add(room);

            l.room = roomRect;
            l.roomId = id;

            CarveRoom(roomRect, id);
        }

        // Corridors (гарантируем связность)
        CreateCorridors(root);

        // Двери комнат (для закрытия/открытия)
        BuildRoomDoors();

        // Draw tiles
        DrawToTilemaps();
    }

    void InitRandom()
    {
        if (seed == 0)
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        rnd = new System.Random(seed);
    }

    void InitMaps()
    {
        map = new int[width, height];
        roomIdMap = new int[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                map[x, y] = 0;
                roomIdMap[x, y] = -1;
            }
    }

    RectInt CreateRoomInLeaf(RectInt leaf)
    {
        int padding = rnd.Next(1, maxRoomPadding + 1);

        int maxW = Math.Max(minRoomSize, leaf.width - padding * 2);
        int maxH = Math.Max(minRoomSize, leaf.height - padding * 2);

        int roomW = rnd.Next(minRoomSize, maxW + 1);
        int roomH = rnd.Next(minRoomSize, maxH + 1);

        int roomX = rnd.Next(leaf.xMin + padding, (leaf.xMax - padding) - roomW + 1);
        int roomY = rnd.Next(leaf.yMin + padding, (leaf.yMax - padding) - roomH + 1);

        return new RectInt(roomX, roomY, roomW, roomH);
    }

    void CarveRoom(RectInt r, int id)
    {
        for (int y = r.yMin; y < r.yMax; y++)
            for (int x = r.xMin; x < r.xMax; x++)
            {
                map[x, y] = 1;
                roomIdMap[x, y] = id;
            }
    }

    void CreateCorridors(Leaf leaf)
    {
        if (leaf == null) return;

        if (leaf.left != null && leaf.right != null)
        {
            CreateCorridors(leaf.left);
            CreateCorridors(leaf.right);

            var leftRoomLeaf = leaf.left.GetRoomLeaf();
            var rightRoomLeaf = leaf.right.GetRoomLeaf();
            if (leftRoomLeaf == null || rightRoomLeaf == null) return;

            Vector2Int a = Center(leftRoomLeaf.room);
            Vector2Int b = Center(rightRoomLeaf.room);

            CarveCorridor(a, b);
        }
    }

    Vector2Int Center(RectInt r) => new Vector2Int(r.xMin + r.width / 2, r.yMin + r.height / 2);

    void CarveCorridor(Vector2Int a, Vector2Int b)
    {
        // L-образный коридор без диагоналей, ширина 1
        bool xFirst = rnd.NextDouble() < 0.5;

        if (xFirst)
        {
            CarveLineX(a.x, b.x, a.y);
            CarveLineY(a.y, b.y, b.x);
        }
        else
        {
            CarveLineY(a.y, b.y, a.x);
            CarveLineX(a.x, b.x, b.y);
        }
    }

    void CarveLineX(int x0, int x1, int y)
    {
        int from = Math.Min(x0, x1);
        int to = Math.Max(x0, x1);
        for (int x = from; x <= to; x++)
        {
            if (InBounds(x, y))
                map[x, y] = 1; // коридор: roomIdMap остается -1
        }
    }

    void CarveLineY(int y0, int y1, int x)
    {
        int from = Math.Min(y0, y1);
        int to = Math.Max(y0, y1);
        for (int y = from; y <= to; y++)
        {
            if (InBounds(x, y))
                map[x, y] = 1;
        }
    }

    bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    void BuildRoomDoors()
    {
        roomDoors = new List<Vector3Int>[rooms.Count];
        for (int i = 0; i < rooms.Count; i++)
            roomDoors[i] = new List<Vector3Int>();

        // дверь = клетка коридора (-1), которая рядом с клеткой комнаты (>=0)
        for (int y = 1; y < height - 1; y++)
            for (int x = 1; x < width - 1; x++)
            {
                if (map[x, y] != 1) continue;
                if (roomIdMap[x, y] != -1) continue; // нам нужны только клетки коридора

                TryAddDoorCell(x, y, x + 1, y);
                TryAddDoorCell(x, y, x - 1, y);
                TryAddDoorCell(x, y, x, y + 1);
                TryAddDoorCell(x, y, x, y - 1);
            }
    }

    void TryAddDoorCell(int cx, int cy, int nx, int ny)
    {
        int rid = roomIdMap[nx, ny];
        if (rid < 0) return;

        var cell = new Vector3Int(cx, cy, 0);
        if (!roomDoors[rid].Contains(cell))
            roomDoors[rid].Add(cell);
    }

    void DrawToTilemaps()
    {
        if (ground == null || walls == null) return;
        if (floorTiles == null || floorTiles.Length == 0) return;
        if (wallTiles == null || wallTiles.Length == 0) return;

        ground.ClearAllTiles();
        walls.ClearAllTiles();

        var bounds = new BoundsInt(0, 0, 0, width, height, 1);
        TileBase[] gArr = new TileBase[width * height];
        TileBase[] wArr = new TileBase[width * height];

        int i = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++, i++)
            {
                if (map[x, y] == 1)
                    gArr[i] = floorTiles[rnd.Next(0, floorTiles.Length)];
                else
                    wArr[i] = wallTiles[rnd.Next(0, wallTiles.Length)];
            }

        ground.SetTilesBlock(bounds, gArr);
        walls.SetTilesBlock(bounds, wArr);
    }

    class Leaf
    {
        public RectInt rect;
        public Leaf left, right;

        public RectInt room;
        public int roomId = -1;

        public Leaf(RectInt r)
        {
            rect = r;
            room = new RectInt();
        }

        public bool Split(System.Random rnd, int minLeafSize)
        {
            bool splitH = rnd.NextDouble() > 0.5;

            if (rect.width > rect.height && rect.width / (float)rect.height >= 1.25f) splitH = false;
            else if (rect.height > rect.width && rect.height / (float)rect.width >= 1.25f) splitH = true;

            int max = (splitH ? rect.height : rect.width) - minLeafSize;
            if (max <= minLeafSize) return false;

            int split = rnd.Next(minLeafSize, max + 1);

            if (splitH)
            {
                left = new Leaf(new RectInt(rect.x, rect.y, rect.width, split));
                right = new Leaf(new RectInt(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                left = new Leaf(new RectInt(rect.x, rect.y, split, rect.height));
                right = new Leaf(new RectInt(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }

        public Leaf GetRoomLeaf()
        {
            if (roomId >= 0 && room.width > 0) return this;

            Leaf found = null;
            if (left != null) found = left.GetRoomLeaf();
            if (found != null) return found;

            if (right != null) found = right.GetRoomLeaf();
            return found;
        }
    }
}
