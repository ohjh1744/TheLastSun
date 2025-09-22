using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    private MonsterModel _model;

    private PooledObject _pooledObject => GetComponent<PooledObject>();

    private void Awake()
    {
        _model = GetComponent<MonsterModel>();
    }

    private void Start()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveX(3, 3f).SetEase(Ease.Linear))
                .Append(transform.DOMoveY(-3, 3f).SetEase(Ease.Linear))
                .Append(transform.DOMoveX(-3, 3f).SetEase(Ease.Linear))
                .Append(transform.DOMoveY(3, 3f).SetEase(Ease.Linear))
                .OnComplete(() => _pooledObject.ReturnPool());
    }
}