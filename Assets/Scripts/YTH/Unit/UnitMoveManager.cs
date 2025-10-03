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
                _selectedController = hit.collider.GetComponent<UnitController>();
                if (_selectedController != null)
                {
                    _selectedController.BeginManualSelect(); // 공격 억제
                }
            }
        }

        // 이동 목적지 확정
        if (Input.GetMouseButtonUp(0) && _selectedController != null)
        {
            Vector3 movepPoint = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(_cam.transform.position.z)));
            movepPoint.z = _selectedController.transform.position.z;
            _targetPos = movepPoint;
            _selectedController.SetManualMoveTarget(_targetPos); 
            _isMoving = true;
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
