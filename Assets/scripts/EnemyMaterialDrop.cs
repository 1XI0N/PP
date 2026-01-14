using UnityEngine;

public class EnemyMaterialDrop : MonoBehaviour
{
    public Health health;

    public MaterialType material = MaterialType.Metal;
    public int amount = 1;

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
        if (MaterialsWallet.Instance != null)
            MaterialsWallet.Instance.Add(material, amount);
    }
}
