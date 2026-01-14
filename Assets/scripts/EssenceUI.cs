using UnityEngine;
using TMPro;

public class EssenceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private int last = int.MinValue;

    void Awake()
    {
        if (!text) text = GetComponent<TMP_Text>();            // если скрипт на самом Text
        if (!text) text = GetComponentInChildren<TMP_Text>();  // если вдруг на родителе
    }

    void Update()
    {
        if (EssenceManager.Instance == null || text == null) return;

        int v = EssenceManager.Instance.Essence;
        if (v == last) return;

        last = v;
        text.text = v.ToString(); // только цифра
    }
}
