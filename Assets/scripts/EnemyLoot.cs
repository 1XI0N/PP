using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    public Health health;

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health != null) health.Died += OnDied;
    }

    void OnDisable()
    {
        if (health != null) health.Died -= OnDied;
    }

    void OnDied()
    {
        // тут твой лут (души/эссенция/что надо)
    }
}
