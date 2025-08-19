using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private UnitModel _model;

    private bool _isDragging = false;

    private Camera _mainCamera;

    private Vector3 _dragStartWorldPos;

    private LineRenderer _lineRenderer;

    private bool _isMoving = false;

    private Vector3 _moveTarget;

    private void Awake()
    {
        _model = GetComponent<UnitModel>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // MoveTowards로 부드럽게 이동
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _model.MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _moveTarget) < 0.01f)
            {
                transform.position = _moveTarget;
                _isMoving = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;

        // 드래그 시작 위치 저장 (z=0으로 고정)
        Vector3 screenPos = eventData.position;
        _dragStartWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -_mainCamera.transform.position.z));
        _dragStartWorldPos.z = 0f;

        // LineRenderer 동적 추가 및 설정
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.05f;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = Color.green;
            _lineRenderer.endColor = Color.green;
            _lineRenderer.sortingOrder = 100; // 충분히 높은 값
            _lineRenderer.sortingLayerName = "Default"; // 필요시 Sorting Layer 명시
            _lineRenderer.useWorldSpace = true;
        }
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _dragStartWorldPos);
        _lineRenderer.SetPosition(1, _dragStartWorldPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging && _lineRenderer != null)
        {
            Vector3 screenPos = eventData.position;
            Vector3 currentWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -_mainCamera.transform.position.z));
            currentWorldPos.z = 0f;
            _lineRenderer.SetPosition(0, _dragStartWorldPos);
            _lineRenderer.SetPosition(1, currentWorldPos);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
        {
            // 마우스를 뗀 위치를 타겟으로 저장, 이동 시작
            Vector3 screenPos = eventData.position;
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_mainCamera.transform.position.z - transform.position.z)));
            worldPos.z = transform.position.z;
            _moveTarget = worldPos;
            _isMoving = true;
            _isDragging = false;

            // 선 비활성화
            if (_lineRenderer != null)
                _lineRenderer.enabled = false;
        }
    }
}
