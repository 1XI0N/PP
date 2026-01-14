using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
    [Header("Auto refs")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Health health;
    [SerializeField] private PlayerDash dash;

    [Header("Animator params")]
    public string moveXParam = "MoveX";
    public string moveYParam = "MoveY";
    public string speedParam = "Speed";
    public string isDeadParam = "IsDead";
    public string isDashingParam = "IsDashing";

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!health) health = GetComponent<Health>();
        if (!dash) dash = GetComponent<PlayerDash>();

        // �����: Animator ����� �� child
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!anim) return;

        Vector2 v = rb ? rb.linearVelocity : Vector2.zero;

        // ��������
        anim.SetFloat(speedParam, v.magnitude);

        // ����������� (���� ����� � �� �������)
        if (v.sqrMagnitude > 0.001f)
        {
            anim.SetFloat(moveXParam, v.x);
            anim.SetFloat(moveYParam, v.y);
        }

        // dash
        if (!string.IsNullOrEmpty(isDashingParam))
        {
            bool d = dash != null && dash.IsDashing; // ������� �������� ����
            anim.SetBool(isDashingParam, d);
        }

        // ������
        if (!string.IsNullOrEmpty(isDeadParam) && health != null)
            anim.SetBool(isDeadParam, health.IsDead);
    }
}

