using UnityEngine;

public class UnitMoveManager2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // ���� �̵� �ӵ�

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
        // ���콺 Ŭ�� �� ���� ���� ���� (2D)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // ���� �ð�ȭ (������)
            Debug.DrawRay(mousePos, Vector2.zero, Color.red, 1f);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Unit Selected!");
                startPosition = hit.collider.gameObject.transform.position;
                target = hit.collider.gameObject;
            }
        }

        // �巡�� ���� �� �̵��� ��ġ ����
        if (Input.GetMouseButtonUp(0) && target != null)
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // ���� �ð�ȭ (�Ķ���)
            Debug.DrawRay(mousePos, Vector2.zero, Color.blue, 1f);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                endPosition = hit.point;
                isMoving = true;
                Debug.Log("Move To: " + endPosition);

                // �浹 ���� ǥ�� (�ʷϻ�)
                Debug.DrawLine(mousePos, hit.point, Color.green, 1f);
            }
            else
            {
                // �� ���� ���̾� ���� �׳� Ŭ�� ��ġ�� �̵�
                endPosition = mousePos;
                isMoving = true;
            }
        }

        // �̵� ����
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
                target = null; // �̵� �Ϸ� �� Ÿ�� �ʱ�ȭ
            }
        }
    }
}
