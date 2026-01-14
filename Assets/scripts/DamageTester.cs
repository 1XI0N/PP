using UnityEngine;

public class DamageTester : MonoBehaviour
{
    private Health hp;

    void Awake()
    {
        hp = GetComponent<Health>();
        Debug.Log("DamageTester Awake on " + name);
    }

    [ContextMenu("TEST: Deal 5 damage")]
    public void TestDamage()
    {
        Debug.Log("TEST: Deal 5 damage called");
        if (hp == null) hp = GetComponent<Health>();

        if (hp == null)
        {
            Debug.LogError("No Health on this object!");
            return;
        }

        hp.TakeDamage(5f);
    }
}

