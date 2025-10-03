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

    private Camera _cam;
    private Vector3 _lastDragWorldPos;

    private void Awake()
    {
        _attackRange = transform.Find("AttackRange")?.gameObject;
        if (_attackRange != null)
            _attackRange.transform.localScale = Vector3.one * _model.AttackRange * 2;
        _attackRange.SetActive(false);


        _cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _attackRange.SetActive(true);
        _controller.BeginManualSelect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 targetPos = new Vector3(eventData.position.x, eventData.position.y, Mathf.Abs(_cam.transform.position.z - transform.position.z));
        _lastDragWorldPos = _cam.ScreenToWorldPoint(targetPos);
        _lastDragWorldPos.z = transform.position.z;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _attackRange.SetActive(false);

        Vector3 targetPos = new Vector3(eventData.position.x, eventData.position.y, Mathf.Abs(_cam.transform.position.z - transform.position.z));
        Vector3 releaseWorldPos = _cam.ScreenToWorldPoint(targetPos);
        releaseWorldPos.z = transform.position.z;

        _controller.SetManualMoveTarget(releaseWorldPos);
    }
}
