using UnityEngine;

public class RoomBoundsBuilder : MonoBehaviour
{
    [Header("Room size in tiles")]
    public int widthTiles = 40;
    public int heightTiles = 25;
    public float tileSize = 1f;

    [Header("Wall thickness (world units)")]
    public float thickness = 1f;

    [Header("Optional: assign layer for walls")]
    public string wallsLayerName = "Walls";

    public void Build()
    {
        // Чистим старые коллайдеры
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        float w = widthTiles * tileSize;
        float h = heightTiles * tileSize;

        // Комната с центром в (0,0) или где стоит генератор?
        // Предположим, что центр комнаты = transform.position.
        Vector2 center = transform.position;

        int wallLayer = LayerMask.NameToLayer(wallsLayerName);

        // Left
        CreateWall("Wall_Left",
            center + new Vector2(-w / 2f - thickness / 2f, 0f),
            new Vector2(thickness, h + thickness * 2f),
            wallLayer);

        // Right
        CreateWall("Wall_Right",
            center + new Vector2(w / 2f + thickness / 2f, 0f),
            new Vector2(thickness, h + thickness * 2f),
            wallLayer);

        // Bottom
        CreateWall("Wall_Bottom",
            center + new Vector2(0f, -h / 2f - thickness / 2f),
            new Vector2(w + thickness * 2f, thickness),
            wallLayer);

        // Top
        CreateWall("Wall_Top",
            center + new Vector2(0f, h / 2f + thickness / 2f),
            new Vector2(w + thickness * 2f, thickness),
            wallLayer);
    }

    void CreateWall(string name, Vector2 pos, Vector2 size, int layer)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.position = pos;

        if (layer != -1) go.layer = layer;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;
    }
}

