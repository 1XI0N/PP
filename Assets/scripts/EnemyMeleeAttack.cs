using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public Transform target;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float cooldown = 1.0f;
    public float damage = 2f;

    float cd;

    void Update()
    {
        if (!target) return;

        cd -= Time.deltaTime;
        if (cd > 0f) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange) return;

        var hp = target.GetComponent<Health>();
        if (hp == null) hp = target.GetComponentInParent<Health>();

        if (hp != null && !hp.IsDead)
        {
            hp.TakeDamage(damage);
            cd = cooldown;
        }
    }
}
