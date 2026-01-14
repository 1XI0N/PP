using UnityEngine;

using System.Collections;

public class PlayerAutoAttack : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CharacterStats stats;
    [SerializeField] private PlayerAnimSideOnly animSide;
    [SerializeField] private Health selfHealth;

    [Header("Targeting")]
    public LayerMask enemyMask;        // поставь Enemy
    public float aggroRange = 3.0f;    // дистанция поиска цели
    public float attackRange = 1.2f;   // дистанция удара
    public float targetRefresh = 0.15f;

    [Header("Hit")]
    public float windup = 0.12f;       // задержка перед ударом
    public float hitRadius = 0.7f;     // радиус области удара
    public float hitOffset = 0.8f;     // смещение вперёд по X (меч)

    private Transform target;
    private float cd;
    private bool attacking;

    void Awake()
    {
        if (!stats) stats = GetComponent<CharacterStats>();
        if (!animSide) animSide = GetComponent<PlayerAnimSideOnly>();
        if (!selfHealth) selfHealth = GetComponent<Health>();
    }

    void Start()
    {
        StartCoroutine(TargetRoutine());
    }

    IEnumerator TargetRoutine()
    {
        var wait = new WaitForSeconds(targetRefresh);

        while (true)
        {
            if (selfHealth != null && selfHealth.IsDead)
                yield break;

            target = FindNearestEnemy();
            yield return wait;
        }
    }

    void Update()
    {
        if (selfHealth != null && selfHealth.IsDead) return;
        if (attacking) return;

        cd -= Time.deltaTime;
        if (cd > 0f) return;
        if (!target) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange) return;

        // атака только влево/вправо: определяем сторону по X относительно цели
        float dirX = target.position.x >= transform.position.x ? 1f : -1f;
        StartCoroutine(AttackRoutine(dirX));
    }

    IEnumerator AttackRoutine(float dirX)
    {
        attacking = true;

        animSide?.TriggerAttackSide(dirX);

        // "замах"
        yield return new WaitForSeconds(windup);

        DoHit(dirX);

        // кулдаун
        float baseCd = stats != null ? stats.AttackCooldown : 0.8f;
        cd = Mathf.Max(0.05f, baseCd);

        attacking = false;
    }

    void DoHit(float dirX)
    {
        float dmg = stats != null ? stats.Damage : 1f;

        Vector2 center = (Vector2)transform.position + new Vector2(Mathf.Sign(dirX) * hitOffset, 0f);

        var hits = Physics2D.OverlapCircleAll(center, hitRadius, enemyMask);
        if (hits == null || hits.Length == 0) return;

        // ударим первого живого
        for (int i = 0; i < hits.Length; i++)
        {
            var hp = hits[i].GetComponentInParent<Health>();
            if (hp == null || hp.IsDead) continue;

            hp.TakeDamage(dmg);
            break;
        }
    }

    Transform FindNearestEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, aggroRange, enemyMask);
        if (cols == null || cols.Length == 0) return null;

        float best = float.MaxValue;
        Transform bestT = null;

        for (int i = 0; i < cols.Length; i++)
        {
            Transform t = cols[i].transform;
            float d = ((Vector2)t.position - (Vector2)transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestT = t;
            }
        }

        return bestT;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}

