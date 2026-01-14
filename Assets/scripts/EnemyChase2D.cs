using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CharacterStats stats;
    [SerializeField] private Health selfHealth;

    [Header("Target")]
    public Transform target;

    [Header("Chase")]
    public float stopDistance = 1.0f;


    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!stats) stats = GetComponent<CharacterStats>();
        if (!selfHealth) selfHealth = GetComponent<Health>();
    }

    void FixedUpdate()
    {
        if (!target) { rb.linearVelocity = Vector2.zero; return; }

        Vector2 to = (Vector2)target.position - rb.position;
        float dist = to.magnitude;

        if (dist <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = stats != null ? stats.MoveSpeed : 2f;
        rb.linearVelocity = to.normalized * speed;
    }


    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
