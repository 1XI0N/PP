using UnityEngine;

public class DeathCleanup : MonoBehaviour
{
    public Health health;
    public float destroyDelay = 1.0f;

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
        Destroy(gameObject, destroyDelay);
    }
}
