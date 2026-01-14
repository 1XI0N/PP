using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDebug : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("BUTTON POINTER CLICK: " + name);
    }
}

