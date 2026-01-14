using UnityEngine;

public class ClickRaycaster2D : MonoBehaviour
{
    [SerializeField] private LayerMask clickableMask = ~0; // можно ограничить кликаемые слои

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cam = Camera.main;
        if (!cam) return;

        Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(wp.x, wp.y);

        RaycastHit2D hit = Physics2D.Raycast(p, Vector2.zero, 0f, clickableMask);

        if (!hit.collider)
        {
            // Debug.Log("Hit: nothing");
            return;
        }

        // Debug.Log("Hit: " + hit.collider.name);

        var crystal = hit.collider.GetComponentInParent<CrystalClickReward>();
        if (crystal != null)
            crystal.Click();
    }
}

