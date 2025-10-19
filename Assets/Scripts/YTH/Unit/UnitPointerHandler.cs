using System.Collections;
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
    private Vector3 _dragStartPos; // 드래그 시작 위치 저장

    private float _limit_Up = 2.1f;
    private float _limit_Down = -3.6f;
    private float _limit_Right = 2.6f;
    private float _limit_Left = -2.6f;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _attackRange = transform.Find("AttackRange")?.gameObject;

        _ui = transform.childCount > 1 ? transform.GetChild(1).gameObject : null;
        _attackRangeUI = _ui != null ? _ui.GetComponent<RectTransform>() : null;

        _cantMove = transform.childCount > 2 ? transform.GetChild(2).gameObject : null;

        if (_attackRange != null)
        {
            _attackRange.transform.localPosition = Vector3.zero + new Vector3 (0, 0.5f);
            _attackRange.transform.localScale = Vector3.one * _model.AttackRange * 2;
            _attackRange.SetActive(false);
        }

        if (_attackRangeUI != null)
        {
            _attackRangeUI.localPosition = new Vector3(0f, 2f, 0f);
            _attackRangeUI.sizeDelta = new Vector2(2f, 1f);
            if (_ui != null) _ui.SetActive(false);
        }

        if (_cantMove != null)
            _cantMove.SetActive(false);

        _cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InOverlay();

        _dragStartPos = transform.position;
        _controller.BeginManualSelect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 targetPos = new Vector3(eventData.position.x, eventData.position.y, 0);
        _lastDragWorldPos = _cam.ScreenToWorldPoint(targetPos);
        _lastDragWorldPos.z = 0;

        bool inside = IsInsideBounds(_lastDragWorldPos);

        transform.position = _lastDragWorldPos;

        if (inside)
        {
           InOverlay();
        }
        else
        {
            OutOverlay();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 실제 이동 위치 계산
        Vector3 targetPos = new Vector3(eventData.position.x, eventData.position.y, 0);
        Vector3 releaseWorldPos = _cam.ScreenToWorldPoint(targetPos);
        releaseWorldPos.z = 0;

        ResetOverlay();

        if (!IsInsideBounds(releaseWorldPos))
        {
            transform.position = _dragStartPos;

            // 같은 프레임 내 다른 시스템이 이동을 걸어도 무시되도록 프레임 끝에 한 번 더 스냅 + 이동 취소
            StartCoroutine(SnapBackAndCancelMoveEndOfFrame(_dragStartPos));
            return;
        }

        // 경계 안: 현재 위치와 타겟을 모두 경계 내로 클램프 후 이동 시작
        transform.position = ClampToBounds(transform.position);
        Vector3 clampedTarget = ClampToBounds(releaseWorldPos);

        _controller.SetManualMoveTarget(clampedTarget);
    }

    private IEnumerator SnapBackAndCancelMoveEndOfFrame(Vector3 pos)
    {
        // 프레임 끝까지 대기 → 다른 Update/입력 처리 이후 최종적으로 적용
        yield return null;
        transform.position = pos;
        // 현재 위치를 타겟으로 지정하여 이동 상태를 즉시 종료
        _controller.SetManualMoveTarget(pos);
    }

    private bool IsInsideBounds(Vector3 pos)
    {
        return pos.x >= _limit_Left && pos.x <= _limit_Right
            && pos.y >= _limit_Down && pos.y <= _limit_Up;
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        float x = Mathf.Clamp(pos.x, _limit_Left, _limit_Right);
        float y = Mathf.Clamp(pos.y, _limit_Down, _limit_Up);
        return new Vector3(x, y, pos.z);
    }

    public void ResetOverlay()
    {
        if (_attackRange != null) _attackRange.SetActive(false);
        if (_ui != null) _ui.SetActive(false);
        if (_cantMove != null) _cantMove.SetActive(false);
        Debug.Log("Reset Overlay");
    }

    public void InOverlay()
    {
        if (_attackRange != null) _attackRange.SetActive(true);
        if (_ui != null) _ui.SetActive(true);
        if (_cantMove != null) _cantMove.SetActive(false);
        Debug.Log("In Overlay");
    }

    public void OutOverlay()
    {
        if (_attackRange != null) _attackRange.SetActive(false);
        if (_ui != null) _ui.SetActive(false);
        if (_cantMove != null) _cantMove.SetActive(true);
        Debug.Log("Out Overlay");
    }
}
