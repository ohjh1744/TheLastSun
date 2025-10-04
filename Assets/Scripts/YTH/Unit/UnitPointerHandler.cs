using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPointerHandler : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IEndDragHandler
{
    private GameObject _attackRange;
    private GameObject _ui;
    private RectTransform _attackRangeUI;

    private UnitModel _model => GetComponent<UnitModel>();
    private UnitController _controller => GetComponent<UnitController>();

    private Camera _cam;
    private Vector3 _lastDragWorldPos;

    private void Awake()
    {
        _attackRange = transform.Find("AttackRange")?.gameObject;

        _ui = transform.GetChild(1).gameObject;
        _attackRangeUI = _ui.GetComponent<RectTransform>();

        if (_attackRange != null)
        {
            _attackRange.transform.localPosition = Vector3.zero;
            _attackRange.transform.localScale = Vector3.one * _model.AttackRange * 2;
            _attackRange.SetActive(false);
        }

        if (_attackRangeUI != null)
        {
            // 요청사항: Position(로컬) (0, 1.5f, 0), Width=2, Height=1
            _attackRangeUI.localPosition = new Vector3(0f, 2f, 0f);
            _attackRangeUI.sizeDelta = new Vector2(2f, 1f);
            _ui.SetActive(false);
        }

        _cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_attackRange != null)
            _attackRange.SetActive(true);

        if (_ui != null)
            _ui.SetActive(true);

        _controller.BeginManualSelect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 targetPos = new Vector3(
            eventData.position.x,
            eventData.position.y,
            Mathf.Abs(_cam.transform.position.z - transform.position.z)
        );

        _lastDragWorldPos = _cam.ScreenToWorldPoint(targetPos);
        _lastDragWorldPos.z = transform.position.z;

        // 드래그 중 포인터 위치를 즉시 따라가도록 이동
        transform.position = _lastDragWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_attackRange != null)
            _attackRange.SetActive(false);

        if (_ui != null)
            _ui.SetActive(false);

        Vector3 targetPos = new Vector3(
            eventData.position.x,
            eventData.position.y,
            Mathf.Abs(_cam.transform.position.z - transform.position.z)
        );
        Vector3 releaseWorldPos = _cam.ScreenToWorldPoint(targetPos);
        releaseWorldPos.z = transform.position.z;

        _controller.SetManualMoveTarget(releaseWorldPos);
    }
}
