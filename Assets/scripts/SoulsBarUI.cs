using UnityEngine;
using UnityEngine.UI;

public class SoulsBarUI : MonoBehaviour
{
    public SoulsWallet wallet;
    public Image fill;

    [Header("How many souls = full bar")]
    public int soulsForFull = 50;

    void Awake()
    {
        if (!fill) fill = GetComponent<Image>();
        if (!wallet) wallet = FindFirstObjectByType<SoulsWallet>();
    }

    void OnEnable()
    {
        if (wallet != null) wallet.Changed += OnChanged;
        Refresh();
    }

    void OnDisable()
    {
        if (wallet != null) wallet.Changed -= OnChanged;
    }

    void OnChanged(int _) => Refresh();

    void Refresh()
    {
        if (!wallet || !fill) return;
        float t = Mathf.Clamp01(wallet.Souls / (float)Mathf.Max(1, soulsForFull));
        fill.fillAmount = t;
    }
}

