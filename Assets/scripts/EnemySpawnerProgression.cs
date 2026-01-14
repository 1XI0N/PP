using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerProgression : MonoBehaviour
{
    [Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        [Min(0f)] public float weight = 1f;

        [Header("Floor availability")]
        public int minFloor = 1;
        public int maxFloor = 999;
    }

    [Header("Enemy catalog")]
    public List<EnemyEntry> enemies = new();

    [Header("Spawn scaling")]
    public int baseCount = 4;
    public int extraPerFloor = 2;

    [Header("Spawn placement")]
    public Vector2 roomCenter = Vector2.zero;
    public Vector2 roomSize = new Vector2(40f, 25f);
    public float padding = 2f;

    [Header("Optional: parent for spawned enemies")]
    public Transform enemiesRoot;

    public event Action AllEnemiesDead;

    //  ВОТ ОН, из-за него у тебя "alive не существует"
    private readonly List<Health> alive = new();
    private readonly Dictionary<Health, Action> diedHandlers = new();

    public void SpawnForFloor(int floor, Transform player)
    {
        // чистим мусор
        alive.RemoveAll(h => h == null);

        int count = Mathf.Max(1, baseCount + extraPerFloor * (floor - 1));
        Debug.Log($"[Spawner] SpawnForFloor floor={floor} count={count} enemiesList={enemies.Count}");

        for (int i = 0; i < count; i++)
        {
            var prefab = PickRandomPrefabForFloor(floor);
            if (prefab == null)
            {
                Debug.LogWarning($"[Spawner] No prefab for floor={floor} (Missing or not allowed by min/max).");
                break;
            }

            Vector2 pos = RandomPointInRoom();
            GameObject go = Instantiate(prefab, pos, Quaternion.identity, enemiesRoot);

            var chase = go.GetComponent<EnemyChase2D>();
            if (chase) chase.target = player;

            var atk = go.GetComponent<EnemyMeleeAttack>();
            if (atk) atk.target = player;

            var hp = go.GetComponent<Health>();
            if (hp == null) hp = go.GetComponentInChildren<Health>();

            Debug.Log($"[Spawner] Spawned {go.name} at {pos}. Health={(hp != null ? "YES" : "NO")}");

            if (hp != null)
            {
                alive.Add(hp);

                //  подписка на Died (Action без параметров) + правильная отписка
                Action handler = null;
                handler = () => OnEnemyDied(hp);

                diedHandlers[hp] = handler;
                hp.Died += handler;
            }
        }

        Debug.Log($"[Spawner] Alive tracked after spawn: {alive.Count}");

        if (alive.Count == 0)
        {
            Debug.Log("[Spawner] No alive enemies tracked -> event fired");
            AllEnemiesDead?.Invoke();
        }
    }

    void OnEnemyDied(Health h)
    {
        if (h != null)
        {
            if (diedHandlers.TryGetValue(h, out var handler))
            {
                h.Died -= handler;
                diedHandlers.Remove(h);
            }
        }

        alive.Remove(h);
        Debug.Log($"[Spawner] Enemy died. Alive left: {alive.Count}");

        if (alive.Count <= 0)
        {
            Debug.Log("[Spawner] All enemies dead -> event fired");
            AllEnemiesDead?.Invoke();
        }
    }

    GameObject PickRandomPrefabForFloor(int floor)
    {
        List<EnemyEntry> pool = null;
        float total = 0f;

        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e == null) continue;
            if (e.prefab == null) continue;
            if (e.weight <= 0f) continue;
            if (floor < e.minFloor || floor > e.maxFloor) continue;

            pool ??= new List<EnemyEntry>();
            pool.Add(e);
            total += e.weight;
        }

        if (pool == null || pool.Count == 0 || total <= 0f)
            return null;

        float r = UnityEngine.Random.value * total;
        float acc = 0f;

        for (int i = 0; i < pool.Count; i++)
        {
            acc += pool[i].weight;
            if (r <= acc) return pool[i].prefab;
        }

        return pool[pool.Count - 1].prefab;
    }

    Vector2 RandomPointInRoom()
    {
        float halfX = roomSize.x * 0.5f;
        float halfY = roomSize.y * 0.5f;

        float x = UnityEngine.Random.Range(roomCenter.x - halfX + padding, roomCenter.x + halfX - padding);
        float y = UnityEngine.Random.Range(roomCenter.y - halfY + padding, roomCenter.y + halfY - padding);

        return new Vector2(x, y);
    }
}
