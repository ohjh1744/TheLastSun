using UnityEngine;

/// <summary>
/// 여러 유닛을 씬에서 선택/이동시키는 전역형 컨트롤(선택된 유닛 이동 중 공격 금지)
/// </summary>
public class UnitMoveManager2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Camera _cam;
    private UnitController _selectedController;
    private Vector3 _targetPos;
    private bool _isMoving = false;

    // 이번 마우스 프레스에서 실제로 유닛을 선택했는지 여부
    private bool _hasActivePress = false;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        // 유닛 선택
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(_cam.transform.position.z)));
            Vector2 mp2 = mp;
            RaycastHit2D hit = Physics2D.Raycast(mp2, Vector2.zero);

            if (hit.collider != null)
            {
                var ctrl = hit.collider.GetComponent<UnitController>();
                if (ctrl != null)
                {
                    _selectedController = ctrl;
                    _selectedController.BeginManualSelect(); // 공격 억제
                    _hasActivePress = true; // 이번 프레스에서 선택됨
                }
                else
                {
                    // 유닛이 아닌 다른 콜라이더
                    _selectedController = null;
                    _isMoving = false;
                    _hasActivePress = false;
                }
            }
            else
            {
                // 빈 공간 클릭: 선택 해제
                _selectedController = null;
                _isMoving = false;
                _hasActivePress = false;
            }
        }

        // 이동 목적지 확정 (이번 프레스에서 선택된 경우에만)
        if (Input.GetMouseButtonUp(0) && _selectedController != null && _hasActivePress)
        {
            Vector3 movepPoint = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(_cam.transform.position.z)));
            movepPoint.z = _selectedController.transform.position.z;
            _targetPos = movepPoint;

            _selectedController.SetManualMoveTarget(_targetPos);
            _isMoving = true;

            // 이번 프레스 종료
            _hasActivePress = false;
        }

        if (_isMoving && _selectedController != null)
        {
            float dist = Vector3.Distance(_selectedController.transform.position, _targetPos);
            if (dist < 0.06f)
            {
                _isMoving = false;
                _selectedController = null;
            }
        }
    }
}
