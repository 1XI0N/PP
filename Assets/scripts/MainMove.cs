using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;                 // ��������� ����������
        rb.freezeRotation = true;             // ��������� ��������
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // ������������� ������������� ����
    }

    void Update()
    {
        // �������� ���� � ����������
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize(); // ���������� �������� �� ���������
    }

    void FixedUpdate()
    {
        // �������� ����� Rigidbody2D
        rb.linearVelocity = input * speed;
    }
}
