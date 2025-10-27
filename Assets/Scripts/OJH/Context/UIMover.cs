using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private float _xMove;

    [SerializeField] private float _yMove; // 50

    [SerializeField] private float _moveTime; // 1

    [SerializeField] private float _pauseDuration; // 반복 주기 사이의 텀

    void Start()
    {
        Vector2 originalPos = _rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(originalPos.x + _xMove, originalPos.y + _yMove);

        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Append(_rectTransform.DOAnchorPos(targetPos, _moveTime).SetEase(Ease.Linear))
                    .Append(_rectTransform.DOAnchorPos(originalPos, _moveTime).SetEase(Ease.Linear))
                    .AppendInterval(_pauseDuration)
                    .SetLoops(-1);
    }


}
