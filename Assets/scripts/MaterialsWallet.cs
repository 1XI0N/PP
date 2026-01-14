using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsWallet : MonoBehaviour
{
    public static MaterialsWallet Instance { get; private set; }

    [Serializable]
    public class Entry
    {
        public MaterialType type;
        public int amount;
    }

    [SerializeField] private List<Entry> entries = new();

    public event Action Changed;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int Get(MaterialType type)
    {
        var e = entries.Find(x => x.type == type);
        return e != null ? e.amount : 0;
    }

    public void Add(MaterialType type, int amount)
    {
        if (amount <= 0) return;
        var e = entries.Find(x => x.type == type);
        if (e == null)
        {
            e = new Entry { type = type, amount = 0 };
            entries.Add(e);
        }
        e.amount += amount;
        Changed?.Invoke();
    }

    public bool Spend(MaterialType type, int amount)
    {
        if (amount <= 0) return true;
        var e = entries.Find(x => x.type == type);
        if (e == null || e.amount < amount) return false;

        e.amount -= amount;
        Changed?.Invoke();
        return true;
    }

    public bool Has(MaterialType type, int amount) => Get(type) >= amount;
}

