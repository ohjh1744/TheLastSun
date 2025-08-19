using UnityEngine;
using UnityEngine.EventSystems;

public class MoveHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private UnitModel _model;

    RaycastHit2D hit;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Drag ���� �� �̵� ���� ���� Ȱ��ȭ
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Drag ���� �� �̵� ���� ���� ��Ȱ��ȭ
        // �̵� ��� ó��
        if (hit.collider != null)
        {
            Vector2 targetPosition = hit.point;
            transform.Translate(
                targetPosition - (Vector2)transform.position,
                Space.World
            );
        }
    }
}
