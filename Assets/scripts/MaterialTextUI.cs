using TMPro;
using UnityEngine;

public class MaterialTextUI : MonoBehaviour
{
    public MaterialType type;
    public TMP_Text text;

    void Awake()
    {
        if (!text) text = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (MaterialsWallet.Instance != null)
            MaterialsWallet.Instance.Changed += Refresh;

        Refresh();
    }

    void OnDisable()
    {
        if (MaterialsWallet.Instance != null)
            MaterialsWallet.Instance.Changed -= Refresh;
    }

    void Refresh()
    {
        if (!text) return;

        int v = (MaterialsWallet.Instance != null) ? MaterialsWallet.Instance.Get(type) : 0;
        text.text = v.ToString(); // только цифра
    }
}

