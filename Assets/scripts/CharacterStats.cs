using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Runtime stats")]
    [SerializeField] private float maxHp = 10f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackCooldown = 0.9f;
    [SerializeField] private float armor = 0f;

    public float MaxHp => maxHp;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackCooldown => attackCooldown;
    public float Armor => armor;

    // ? Методы для изменения статов извне (без сеттеров)
    public void SetMaxHp(float value) => maxHp = Mathf.Max(1f, value);
    public void SetDamage(float value) => damage = Mathf.Max(0.1f, value);
    public void SetMoveSpeed(float value) => moveSpeed = Mathf.Max(0.1f, value);
    public void SetAttackCooldown(float value) => attackCooldown = Mathf.Max(0.05f, value);
    public void SetArmor(float value) => armor = Mathf.Max(0f, value);
}
