using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MouseHoverHandler : MonoBehaviour, IPointerEnterHandler
{
    public Action<PointerEventData> OnHoverHandler = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnHoverHandler != null)
            OnHoverHandler.Invoke(eventData);
    }
}
