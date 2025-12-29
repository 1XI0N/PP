using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomFlowController : MonoBehaviour
{
    [Header("Refs")]
    public RoguelikeLevelGenerator gen;
    public RoomTracker tracker;
    public RoomContentSpawner spawner;

    [Header("Tilemaps")]
    public Tilemap ground;
    public Tilemap walls;
    public Tilemap special;

    [Header("Tiles")]
    public TileBase[] floorTiles;
    public TileBase wallTile;
    public TileBase portalTile;

    [Header("Player")]
    public Transform player;
    public PlayerHealth playerHealth;

    public event Action<int> RoomCleared;
    public event Action PlayerDied;

    int currentRoomId = -1;
    int aliveEnemies = 0;

    bool[] roomStarted;
    bool[] roomCleared;

    // где стоит портал в каждой комнате (чтобы корректно удалять/обновлять)
    Dictionary<int, Vector3Int> portalCellByRoom = new Dictionary<int, Vector3Int>();

    void Start()
    {
        RebuildState();

        if (tracker != null)
            tracker.OnRoomEnter += HandleEnterRoom;

        if (playerHealth != null)
            playerHealth.Died += HandlePlayerDied;
    }

    void OnDestroy()
    {
        if (tracker != null)
            tracker.OnRoomEnter -= HandleEnterRoom;

        if (playerHealth != null)
            playerHealth.Died -= HandlePlayerDied;
    }

    void RebuildState()
    {
        if (gen == null) return;

        roomStarted = new bool[gen.rooms.Count];
        roomCleared = new bool[gen.rooms.Count];
        portalCellByRoom.Clear();
        aliveEnemies = 0;
        currentRoomId = -1;
    }

    public void ResetForNewLevel()
    {
        // На всякий случай очищаем спец-тайлы
        if (special != null)
            special.ClearAllTiles();

        RebuildState();
    }

    void HandleEnterRoom(int roomId)
    {
        if (gen == null || roomId < 0 || roomId >= gen.rooms.Count) return;

        // если уровень только что перегенерился и массивы не совпали
        if (roomStarted == null || roomStarted.Length != gen.rooms.Count)
            RebuildState();

        currentRoomId = roomId;

        // Если комната уже была очищена — оставляем открытой и с порталом (можно не менять)
        if (roomCleared[roomId])
        {
            UnlockRoom(roomId);
            ShowPortal(roomId);
            return;
        }

        // Если уже начинали — не перезапускаем спавн/закрытие
        if (roomStarted[roomId]) return;

        roomStarted[roomId] = true;

        LockRoom(roomId);
        HidePortal(roomId);

        // спавним врагов и подписываемся на их смерть
        aliveEnemies = 0;
        var enemies = spawner != null ? spawner.SpawnEnemiesForRoom(roomId) : new List<EnemyHealth>();

        foreach (var e in enemies)
        {
            if (e == null) continue;
            aliveEnemies++;
            e.Died += HandleEnemyDied;
        }

        // если монстров нет — сразу очищаем
        if (aliveEnemies == 0)
            ClearRoom(roomId);
    }

    void HandleEnemyDied(EnemyHealth e)
    {
        if (e != null)
            e.Died -= HandleEnemyDied;

        aliveEnemies--;

        if (aliveEnemies <= 0 && currentRoomId >= 0 && !roomCleared[currentRoomId])
            ClearRoom(currentRoomId);
    }

    void ClearRoom(int roomId)
    {
        roomCleared[roomId] = true;

        UnlockRoom(roomId);
        ShowPortal(roomId);

        RoomCleared?.Invoke(roomId);
    }

    void HandlePlayerDied()
    {
        // закрываем текущую комнату и убираем портал
        if (currentRoomId >= 0 && currentRoomId < gen.rooms.Count)
        {
            HidePortal(currentRoomId);
            LockRoom(currentRoomId);
        }

        PlayerDied?.Invoke();
    }

    void LockRoom(int roomId)
    {
        if (gen.roomDoors == null || gen.roomDoors.Length == 0) return;
        if (walls == null || ground == null) return;
        if (wallTile == null) return;

        foreach (var doorCell in gen.roomDoors[roomId])
        {
            walls.SetTile(doorCell, wallTile);
            ground.SetTile(doorCell, null);
        }
    }

    void UnlockRoom(int roomId)
    {
        if (gen.roomDoors == null || gen.roomDoors.Length == 0) return;
        if (walls == null || ground == null) return;
        if (floorTiles == null || floorTiles.Length == 0) return;

        foreach (var doorCell in gen.roomDoors[roomId])
        {
            walls.SetTile(doorCell, null);
            ground.SetTile(doorCell, floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)]);
        }
    }

    void ShowPortal(int roomId)
    {
        if (portalTile == null || special == null || gen == null) return;

        // выбираем клетку портала так, чтобы она не совпала с клеткой игрока (по возможности)
        Vector3Int portalCell = PickPortalCell(roomId);

        // если был старый портал в этой комнате — убираем
        HidePortal(roomId);

        special.SetTile(portalCell, portalTile);
        portalCellByRoom[roomId] = portalCell;
    }

    void HidePortal(int roomId)
    {
        if (special == null || portalTile == null) return;

        if (portalCellByRoom.TryGetValue(roomId, out var oldCell))
        {
            if (special.GetTile(oldCell) == portalTile)
                special.SetTile(oldCell, null);

            portalCellByRoom.Remove(roomId);
        }
    }

    Vector3Int PickPortalCell(int roomId)
    {
        var room = gen.rooms[roomId];

        Vector3Int playerCell = new Vector3Int(int.MinValue, int.MinValue, 0);
        if (player != null && ground != null)
            playerCell = ground.WorldToCell(player.position);

        // попробуем выбрать клетку подальше от игрока, чтобы он не стоял на портале сразу
        const int MIN_MANHATTAN = 3;

        for (int tries = 0; tries < 200; tries++)
        {
            int x = UnityEngine.Random.Range(room.rect.xMin, room.rect.xMax);
            int y = UnityEngine.Random.Range(room.rect.yMin, room.rect.yMax);

            if (gen.roomIdMap[x, y] != roomId) continue;

            var c = new Vector3Int(x, y, 0);

            int manhattan = Mathf.Abs(c.x - playerCell.x) + Mathf.Abs(c.y - playerCell.y);
            if (manhattan < MIN_MANHATTAN) continue;

            return c;
        }

        // fallback: центр
        return new Vector3Int(room.center.x, room.center.y, 0);
    }
}
