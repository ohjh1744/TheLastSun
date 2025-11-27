using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventReceiver : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerUpHandler
    , IPointerDownHandler
    , IPointerMoveHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IDragHandler
    , IDropHandler
{
    public event UnityAction<PointerEventData> OnClicked;
    public event UnityAction<PointerEventData> OnEntered;
    public event UnityAction<PointerEventData> OnExited;
    public event UnityAction<PointerEventData> OnUped;
    public event UnityAction<PointerEventData> OnDowned;
    public event UnityAction<PointerEventData> OnMoved;
    public event UnityAction<PointerEventData> OnBeginDraged;
    public event UnityAction<PointerEventData> OnEndDraged;
    public event UnityAction<PointerEventData> OnDraged;
    public event UnityAction<PointerEventData> OnDroped;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEntered?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExited?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUped?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDowned?.Invoke(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        OnMoved?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDraged?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDraged?.Invoke(eventData);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDraged?.Invoke(eventData);
    }


    public void OnDrop(PointerEventData eventData)
    {
        OnDroped?.Invoke(eventData);
    }
}