using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICardHoverHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UICard parentCard;
    public void Initialize(UICard uicard)
    {
        parentCard = uicard;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        parentCard.Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        parentCard.Unhighlight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parentCard.Select();
    }
}
