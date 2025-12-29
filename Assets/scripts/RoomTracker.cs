using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTracker : MonoBehaviour
{
    public RoguelikeLevelGenerator generator;
    public Tilemap ground;
    public Transform player;

    public float checkInterval = 0.1f;

    public event Action<int> OnRoomEnter;

    int currentRoom = -1;
    float t;

    void Update()
    {
        if (generator == null || ground == null || player == null) return;

        t += Time.deltaTime;
        if (t < checkInterval) return;
        t = 0f;

        Vector3Int cell = ground.WorldToCell(player.position);

        if (cell.x < 0 || cell.y < 0 || cell.x >= generator.width || cell.y >= generator.height)
            return;

        int id = generator.roomIdMap[cell.x, cell.y];

        if (id >= 0 && id != currentRoom)
        {
            currentRoom = id;
            OnRoomEnter?.Invoke(id);
        }
    }

    public int CurrentRoomId => currentRoom;
}
