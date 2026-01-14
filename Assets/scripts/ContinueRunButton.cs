using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueRunButton : MonoBehaviour
{
    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() =>
        {
            var ctrl = FindFirstObjectByType<RunDungeonController>();
            if (ctrl) ctrl.OnContinuePressed();
            else Debug.LogError("RunDungeonController not found in scene!");
        });
    }
}
