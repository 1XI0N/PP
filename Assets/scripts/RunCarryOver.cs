using UnityEngine;

public class RunCarryOver : MonoBehaviour
{
    public static RunCarryOver Instance { get; private set; }

    public int Souls { get; private set; }
    public float Hp { get; private set; }
    public float MaxHp { get; private set; }

    // следующий этаж, с которого стартуем при новом заходе в данж
    public int NextFloor { get; private set; } = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // подхватим сохранённое между сессиями (не обязательно, но удобно)
        NextFloor = PlayerPrefs.GetInt("NEXT_FLOOR", 1);
    }

    public void SetFromRun(int souls, float hp, float maxHp)
    {
        Souls = Mathf.Max(0, souls);
        MaxHp = Mathf.Max(1f, maxHp);
        Hp = Mathf.Clamp(hp, 0f, MaxHp);
    }

    public void SetNextFloor(int nextFloor)
    {
        NextFloor = Mathf.Max(1, nextFloor);
        PlayerPrefs.SetInt("NEXT_FLOOR", NextFloor);
        PlayerPrefs.Save();
        Debug.Log($"[RunCarryOver] NextFloor set to {NextFloor}");
    }

    public void ResetProgress()
    {
        SetNextFloor(1);
    }

    public void Clear()
    {
        Souls = 0;
        Hp = 0f;
        MaxHp = 1f;
        // прогресс по этажам НЕ трогаем, если не хочешь сбрасывать
    }
}
