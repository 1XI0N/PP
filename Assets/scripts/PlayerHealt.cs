using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event Action Died;

    public int maxHp = 100;
    public int hp = 100;

    public void TakeDamage(int dmg)
    {
        if (hp <= 0) return;
        hp -= dmg;

        if (hp <= 0)
        {
            hp = 0;
            Died?.Invoke();
        }
    }

    public void RestoreFull()
    {
        hp = maxHp;
    }
}
