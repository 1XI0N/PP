using System;
using UnityEngine;

public class EssenceManager : MonoBehaviour
{
    public static EssenceManager Instance { get; private set; }

    [SerializeField] private int essence = 0;
    public int Essence => essence;

    public event Action<int> EssenceChanged; // новое значение

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        essence += amount;
        EssenceChanged?.Invoke(essence);

        Debug.Log($"[EssenceManager] +{amount}, now={essence}");
    }

    public bool Spend(int amount)
    {
        if (amount <= 0) return true;
        if (essence < amount) return false;

        essence -= amount;
        EssenceChanged?.Invoke(essence);

        Debug.Log($"[EssenceManager] -{amount}, now={essence}");
        return true;
    }

    public void Set(int value)
    {
        essence = Mathf.Max(0, value);
        EssenceChanged?.Invoke(essence);
    }
}
