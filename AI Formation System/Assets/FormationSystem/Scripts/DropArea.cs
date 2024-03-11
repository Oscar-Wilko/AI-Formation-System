using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.GetComponent<DragFormation>())
            return;
        if (!InBounds(eventData.pointerDrag.GetComponent<RectTransform>()))
            return;
        GameObject newFormation = Instantiate(eventData.pointerDrag, transform);
        newFormation.name = "DraggedFormation";
        newFormation.GetComponent<RawImage>().raycastTarget = true;
    }

    private bool InBounds(RectTransform rect)
    {
        RectTransform selfRect = GetComponent<RectTransform>();
        if (!selfRect.rect.Contains(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y) + rect.sizeDelta * 0.5f))
            return false;
        if (!selfRect.rect.Contains(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y) - rect.sizeDelta * 0.5f))
            return false;
        return true;
    }
}
