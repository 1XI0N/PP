using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimatorDriver : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Health health;

    [Header("Facing")]
    public Transform target;
    public float flipDeadZone = 0.05f;

    [Header("Animator Params")]
    public string attackingParam = "IsAttacking";
    public string deadParam = "IsDead";

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!health) health = GetComponent<Health>();
    }

    void Update()
    {
        if (!anim) return;
        if (health != null)
            anim.SetBool("IsDead", health.IsDead);
    }

    void LateUpdate()
    {
        if (!sr || !target) return;

        float dx = target.position.x - transform.position.x;
        if (Mathf.Abs(dx) > flipDeadZone)
            sr.flipX = dx < 0f;
    }

    public void SetAttacking(bool value)
    {
        if (!anim) return;
        anim.SetBool(attackingParam, value);
    }
}
