using UnityEngine;
using UnityEngine.Tilemaps;

public class PortalTileStepper : MonoBehaviour
{
    public Tilemap special;
    public TileBase portalTile;
    public Transform player;

    public GameFlowController flow;

    bool triggered;

    void Update()
    {
        if (special == null || portalTile == null || player == null || flow == null) return;

        // Пока открыта табличка — не даем переходить
        if (flow.IsPopupOpen())
        {
            triggered = false;
            return;
        }

        Vector3Int cell = special.WorldToCell(player.position);
        TileBase t = special.GetTile(cell);

        if (t == portalTile)
        {
            if (triggered) return;
            triggered = true;

            // убираем свет, чтобы не сработал повторно
            special.SetTile(cell, null);

            flow.StartNextLevel();
        }
        else
        {
            triggered = false;
        }
    }
}
