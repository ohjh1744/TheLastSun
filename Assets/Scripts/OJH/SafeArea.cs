using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;  // UI 요소의 RectTransform

    private Vector2 _minAnchor;            // anchorMin 값

    private Vector2 _maxAnchor;            // anchorMax 값

    private void Awake()
    {
        if (_rectTransform == null)
        {
            return;
        }

        // SafeZone의 왼쪽 아래 모서리 계산
        _minAnchor.x = Screen.safeArea.position.x / Screen.width;
        _minAnchor.y = Screen.safeArea.position.y / Screen.height;

        //SafeZone의 오른쪽 위 모서리 계산
        _maxAnchor.x = (Screen.safeArea.position.x + Screen.safeArea.size.x) / Screen.width;
        _maxAnchor.y = (Screen.safeArea.position.y + Screen.safeArea.size.y) / Screen.height;

        // 앵커 값을 RectTransform에 적용
        _rectTransform.anchorMin = _minAnchor;
        _rectTransform.anchorMax = _maxAnchor;

        // offset 0으로 설정.
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }
}
