using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPointerHandler : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IEndDragHandler
{
    private GameObject _attackRange;

    private UnitModel _model => GetComponent<UnitModel>();

    private UnitController _controller => GetComponent<UnitController>();

    private void Awake()
    {
        _attackRange = transform.Find("AttackRange").gameObject;
        _attackRange.transform.localScale = Vector3.one * _model.AttackRange;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _attackRange.SetActive(true);
        _controller.CurrentState = State.Idle;
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _attackRange.SetActive(false);
        _controller.CurrentState = State.Idle;
        Debug.Log("OnEndDrag");
    }

   
}
