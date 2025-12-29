using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunChoicePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public Button continueButton;
    public Button baseButton;

    Action onContinue;
    Action onBase;

    void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(() => { Hide(); onContinue?.Invoke(); });

        if (baseButton != null)
            baseButton.onClick.AddListener(() => { Hide(); onBase?.Invoke(); });

        Hide();
    }

    public void Show(string title, int score, Action continueAction, Action baseAction)
    {
        onContinue = continueAction;
        onBase = baseAction;

        if (titleText != null) titleText.text = title;
        if (scoreText != null) scoreText.text = "Очки: " + score;

        if (root != null) root.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
