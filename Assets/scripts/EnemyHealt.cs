using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action<EnemyHealth> Died;

    public int hp = 10;
    public int pointsPerKill = 5;

    public void TakeDamage(int dmg)
    {
        if (hp <= 0) return;
        hp -= dmg;

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    void Die()
    {
        if (ScoreManager.I != null)
            ScoreManager.I.Add(pointsPerKill);

        Died?.Invoke(this);
        Destroy(gameObject);
    }
}
