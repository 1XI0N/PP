using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager I { get; private set; }

    public int RunScore { get; private set; }
    public event Action<int> ScoreChanged;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetRun()
    {
        RunScore = 0;
        ScoreChanged?.Invoke(RunScore);
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        RunScore += amount;
        ScoreChanged?.Invoke(RunScore);
    }
}
