using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CharacterStats stats;

    [Header("Runtime")]
    [SerializeField] private float currentHp = 10f;
    [SerializeField] private bool invulnerable = false;
    [SerializeField] private bool dead = false;

    public float CurrentHp => currentHp;
    public bool IsDead => dead;

    //  событи€ Ѕ≈« параметров (как у теб€ по ошибкам)
    public event Action Changed;
    public event Action Damaged;
    public event Action Died;

    void Awake()
    {
        if (!stats) stats = GetComponent<CharacterStats>();
        ResetToMax();
    }

    public void ResetToMax()
    {
        dead = false;
        float max = stats ? stats.MaxHp : 10f;
        currentHp = Mathf.Max(1f, max);
        Changed?.Invoke();
    }

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;
        if (invulnerable) return;
        if (amount <= 0f) return;

        // если есть брон€ Ч учитываем
        float armor = stats ? stats.Armor : 0f;
        float dmg = Mathf.Max(0f, amount - armor);

        if (dmg <= 0f) return;

        currentHp -= dmg;
        Damaged?.Invoke();
        Changed?.Invoke();

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (dead) return;
        if (amount <= 0f) return;

        float max = stats ? stats.MaxHp : Mathf.Max(1f, currentHp);
        currentHp = Mathf.Min(max, currentHp + amount);
        Changed?.Invoke();
    }

    //  нужно дл€ переносов/лобби
    public void ForceSetHp(float hp)
    {
        float max = stats ? stats.MaxHp : Mathf.Max(1f, hp);
        currentHp = Mathf.Clamp(hp, 0f, max);

        if (currentHp <= 0f)
        {
            Die();
            return;
        }

        dead = false;
        Changed?.Invoke();
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        currentHp = 0f;
        Changed?.Invoke();
        Died?.Invoke();
    }
}
