using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health health;
    public CharacterStats stats;
    public Image fill;

    void Awake()
    {
        if (!fill) fill = GetComponent<Image>();
        if (!health) health = FindFirstObjectByType<Health>();
        if (!stats && health) stats = health.GetComponent<CharacterStats>();
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.Changed += Refresh;
            health.Died += Refresh;
        }
        Refresh();
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.Changed -= Refresh;
            health.Died -= Refresh;
        }
    }

    void Refresh()
    {
        if (!health || !fill) return;

        float maxHp = stats ? stats.MaxHp : Mathf.Max(1f, health.CurrentHp);
        fill.fillAmount = Mathf.Clamp01(health.CurrentHp / Mathf.Max(1f, maxHp));
    }
}
