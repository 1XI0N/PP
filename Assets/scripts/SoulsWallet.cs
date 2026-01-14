using System;
using UnityEngine;

public class SoulsWallet : MonoBehaviour
{
    [SerializeField] private int souls;
    public int Souls => souls;

    public event Action<int> Changed;

    public void Add(int amount)
    {
        if (amount <= 0) return;
        souls += amount;
        Changed?.Invoke(souls);
    }

    public void Set(int value)
    {
        souls = Mathf.Max(0, value);
        Changed?.Invoke(souls);
    }

    public void ResetToZero()
    {
        souls = 0;
        Changed?.Invoke(souls);
    }
}
