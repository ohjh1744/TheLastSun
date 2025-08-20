using UnityEngine;

public class UnitMoveManager2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 유닛 이동 속도

    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isMoving = false;

    private Camera mainCam;

    private GameObject target;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 마우스 클릭 시 유닛 선택 레이 (2D)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // 레이 시각화 (빨간색)
            Debug.DrawRay(mousePos, Vector2.zero, Color.red, 1f);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Unit Selected!");
                startPosition = hit.collider.gameObject.transform.position;
                target = hit.collider.gameObject;
            }
        }

        // 드래그 종료 시 이동할 위치 결정
        if (Input.GetMouseButtonUp(0) && target != null)
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // 레이 시각화 (파란색)
            Debug.DrawRay(mousePos, Vector2.zero, Color.blue, 1f);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                endPosition = hit.point;
                isMoving = true;
                Debug.Log("Move To: " + endPosition);

                // 충돌 지점 표시 (초록색)
                Debug.DrawLine(mousePos, hit.point, Color.green, 1f);
            }
            else
            {
                // 땅 같은 레이어 없이 그냥 클릭 위치로 이동
                endPosition = mousePos;
                isMoving = true;
            }
        }

        // 이동 로직
        if (isMoving && target != null)
        {
            target.transform.position = Vector2.MoveTowards(
                target.transform.position,
                endPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(target.transform.position, endPosition) < 0.05f)
            {
                isMoving = false;
                target = null; // 이동 완료 후 타겟 초기화
            }
        }
    }
}
