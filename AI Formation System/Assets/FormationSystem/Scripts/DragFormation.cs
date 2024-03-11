using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragFormation : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public bool hasDragged;
    public FormationType type;
    public BoxValues boxV;
    public ArrowValues arrV;
    public TriangleValues triV;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!hasDragged)
        {
            GameObject obj = Instantiate(gameObject, transform.parent);
            obj.name = gameObject.name;
        }
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        GetComponent<RawImage>().raycastTarget = false;
        hasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(FormationEditor editor in FindObjectsOfType<FormationEditor>(includeInactive: true))
        {
            editor.gameObject.SetActive(editor.type == type);
            if (editor.type == type)
            {
                editor.Init(this);
            }
        }
    }
}
