using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimSideOnly : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;

    [Header("Animator params")]
    public string attackTrigger = "Attack";
    public string dashTrigger = "Dash";
    public string isDashingBool = "IsDashing";

    private float facingX = 1f; // +1 вправо, -1 влево

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetFacingX(float x)
    {
        if (Mathf.Abs(x) < 0.01f) return;
        facingX = Mathf.Sign(x);
        if (sr) sr.flipX = facingX < 0f;
    }

    public float GetFacingX() => facingX;

    public void TriggerAttackSide(float x)
    {
        SetFacingX(x);
        anim.SetTrigger(attackTrigger);
    }

    public void TriggerDashSide(float x)
    {
        SetFacingX(x);
        anim.SetBool(isDashingBool, true);
        anim.SetTrigger(dashTrigger);
    }

    public void EndDash()
    {
        anim.SetBool(isDashingBool, false);
    }
}

