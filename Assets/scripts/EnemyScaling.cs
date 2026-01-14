using UnityEngine;

public class EnemyScaling : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;

    float baseHp;
    float baseDamage;

    void Awake()
    {
        if (!stats) stats = GetComponent<CharacterStats>();
        baseHp = stats ? stats.MaxHp : 10f;
        baseDamage = stats ? stats.Damage : 1f;
    }

    public void ApplyFloor(int floor, float growth)
    {
        if (!stats) return;
        floor = Mathf.Max(1, floor);
        growth = Mathf.Max(1f, growth);

        float mult = Mathf.Pow(growth, floor - 1);

        // Требуются методы SetMaxHp/SetDamage из твоего CharacterStats (мы их делали)
        stats.SetMaxHp(baseHp * mult);
        stats.SetDamage(baseDamage * mult);
    }
}

