using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    private MonsterModel _model;

    private PooledObject _pooledObject;

    private void Awake()
    {
        _model = GetComponent<MonsterModel>();
        _pooledObject = GetComponent<PooledObject>();
    }

    private void Start()
    {
        // 이동 거리(3 → -3 또는 -3 → 3)는 6, 시간 = 거리 / 속도
        float moveDistance = 6f;
        float moveTime = (_model != null && _model.MoveSpeed > 0) ? moveDistance / _model.MoveSpeed : 3f;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveX(3, moveTime).SetEase(Ease.Linear))
                .Append(transform.DOMoveY(-3, moveTime).SetEase(Ease.Linear))
                .Append(transform.DOMoveX(-3, moveTime).SetEase(Ease.Linear))
                .Append(transform.DOMoveY(3, moveTime).SetEase(Ease.Linear))
                .OnComplete(() => _pooledObject.ReturnPool());
    }
}