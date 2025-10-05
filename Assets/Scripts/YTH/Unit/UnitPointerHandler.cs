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
    private GameObject _cantMove;

    private UnitModel _model => GetComponent<UnitModel>();
    private UnitController _controller => GetComponent<UnitController>();

    private Camera _cam;
    private Vector3 _lastDragWorldPos;
    private Vector3 _lastValidWorldPos; // 마지막 경계 내 유효 위치

    private float _limit_Up = 2.7f;
    private float _limit_Down = -3.7f;
    private float _limit_Right = 3.1f;
    private float _limit_Left = -3.2f;

    private void Awake()
    {
        _attackRange = transform.Find("AttackRange")?.gameObject;

        _ui = transform.GetChild(1).gameObject;
        _attackRangeUI = _ui.GetComponent<RectTransform>();

        _cantMove = transform.GetChild(2).gameObject;

        if (_attackRange != null)
        {
            _attackRange.transform.localPosition = Vector3.zero;
            _attackRange.transform.localScale = Vector3.one * _model.AttackRange * 2;
            _attackRange.SetActive(false);
        }

        if (_attackRangeUI != null)
        {
            _attackRangeUI.localPosition = new Vector3(0f, 2f, 0f);
            _attackRangeUI.sizeDelta = new Vector2(2f, 1f);
            _ui.SetActive(false);
        }

        if (_cantMove != null)
            _cantMove.SetActive(false);

        _cam = Camera.main;
        _lastValidWorldPos = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_attackRange != null) _attackRange.SetActive(true);
        if (_ui != null) _ui.SetActive(true);
        if (_cantMove != null) _cantMove.SetActive(false);

        _lastValidWorldPos = transform.position;
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

        bool inside = IsInsideBounds(_lastDragWorldPos);

        if (inside)
        {
            transform.position = _lastDragWorldPos;
            _lastValidWorldPos = _lastDragWorldPos;

            if (_attackRange != null) _attackRange.SetActive(true);
            if (_ui != null) _ui.SetActive(true);
            if (_cantMove != null) _cantMove.SetActive(false);
        }
        else
        {
            if (_attackRange != null) _attackRange.SetActive(false);
            if (_ui != null) _ui.SetActive(false);
            if (_cantMove != null) _cantMove.SetActive(true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 오버레이 정리
        if (_attackRange != null) _attackRange.SetActive(false);
        if (_ui != null) _ui.SetActive(false);
        if (_cantMove != null) _cantMove.SetActive(false);

        Vector3 targetPos = new Vector3(
            eventData.position.x,
            eventData.position.y,
            Mathf.Abs(_cam.transform.position.z - transform.position.z)
        );
        Vector3 releaseWorldPos = _cam.ScreenToWorldPoint(targetPos);
        releaseWorldPos.z = transform.position.z;

        // 경계 밖에서 마우스 업: 이동 자체를 하지 않음
        if (!IsInsideBounds(releaseWorldPos))
            return;

        // 경계 안: 정상 이동 명령
        _controller.SetManualMoveTarget(releaseWorldPos);
    }

    private bool IsInsideBounds(Vector3 pos)
    {
        return pos.x >= _limit_Left && pos.x <= _limit_Right
            && pos.y >= _limit_Down && pos.y <= _limit_Up;
    }
}
