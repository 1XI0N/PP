using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerAnimSideOnly animSide;
    [SerializeField] private Health health;

    [Header("Dash")]
    public KeyCode dashKey = KeyCode.Space;
    public float dashSpeed = 10f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.6f;

    private float cd;
    private bool dashing;
    public bool IsDashing => dashing;


    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animSide) animSide = GetComponent<PlayerAnimSideOnly>();
        if (!health) health = GetComponent<Health>();
    }

    void Update()
    {
        cd -= Time.deltaTime;

        if (!dashing && cd <= 0f && Input.GetKeyDown(dashKey))
        {
            float x = Input.GetAxisRaw("Horizontal");

            // ����� ������ �� X: ���� �� ������ A/D � ����� � ��������� �������
            float dirX = Mathf.Abs(x) > 0.01f ? Mathf.Sign(x) : animSide.GetFacingX();

            StartCoroutine(DashRoutine(dirX));
        }
    }

    IEnumerator DashRoutine(float dirX)
    {
        dashing = true;
        cd = dashCooldown;

        animSide.TriggerDashSide(dirX);
        if (health != null) health.SetInvulnerable(true);

        float t = 0f;
        Vector2 vel = new Vector2(Mathf.Sign(dirX) * dashSpeed, 0f);

        while (t < dashTime)
        {
            rb.linearVelocity = vel;
            t += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        if (health != null) health.SetInvulnerable(false);
        animSide.EndDash();

        dashing = false;
    }
}

