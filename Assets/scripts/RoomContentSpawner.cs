using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomContentSpawner : MonoBehaviour
{
    public RoguelikeLevelGenerator gen;
    public Tilemap ground;

    [Header("Enemies")]
    public GameObject[] enemyPrefabs;
    public int enemiesMin = 1;
    public int enemiesMax = 3;

    [Header("Runtime Parent (optional)")]
    public Transform runtimeRoot; // если null - создаст сам

    bool[] spawnedRooms;

    void Awake()
    {
        EnsureRuntimeRoot();
    }

    void Start()
    {
        RebuildState();
    }

    void EnsureRuntimeRoot()
    {
        if (runtimeRoot != null) return;

        var go = new GameObject("LevelRuntime");
        runtimeRoot = go.transform;
    }

    void RebuildState()
    {
        if (gen == null) return;
        spawnedRooms = new bool[gen.rooms.Count];
    }

    public void ResetForNewLevel()
    {
        EnsureRuntimeRoot();

        // удалить всех заспавненных объектов
        for (int i = runtimeRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(runtimeRoot.GetChild(i).gameObject);
        }

        // пересоздать массивы под новый набор комнат
        RebuildState();
    }

    public List<EnemyHealth> SpawnEnemiesForRoom(int roomId)
    {
        var result = new List<EnemyHealth>();

        if (gen == null || ground == null) return result;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return result;

        if (spawnedRooms == null || spawnedRooms.Length != gen.rooms.Count)
            RebuildState();

        if (roomId < 0 || roomId >= spawnedRooms.Length) return result;
        if (spawnedRooms[roomId]) return result;

        spawnedRooms[roomId] = true;

        var room = gen.rooms[roomId];
        int count = Random.Range(enemiesMin, enemiesMax + 1);

        int tries = 0;
        while (result.Count < count && tries < 500)
        {
            tries++;

            int x = Random.Range(room.rect.xMin, room.rect.xMax);
            int y = Random.Range(room.rect.yMin, room.rect.yMax);

            if (gen.roomIdMap[x, y] != roomId) continue;

            Vector3 pos = ground.GetCellCenterWorld(new Vector3Int(x, y, 0));
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var go = Instantiate(prefab, pos, Quaternion.identity, runtimeRoot);

            var eh = go.GetComponent<EnemyHealth>();
            if (eh != null) result.Add(eh);
        }

        return result;
    }
}
