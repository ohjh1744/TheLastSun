using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMover : MonoBehaviour
{
    private MonsterModel _model;
    private WaveManager _waveManager;
    private SpriteRenderer _spriteRenderer;
    private Sequence _seq;

    float moveDistance;
    float moveTime;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _model = GetComponent<MonsterModel>();
        _waveManager = GetComponentInParent<WaveManager>();
    }

    private void Start()
    {
        StartMoveSequence();
    }

    private void OnEnable()
    {
        if (_waveManager != null && _waveManager.SpawnPoint != null)
            transform.localPosition = _waveManager.SpawnPoint.localPosition;

        StartMoveSequence();
    }

    private void OnDisable()
    {
        if (_seq != null && _seq.IsActive())
        {
            _seq.Kill();
            _seq = null;
        }
        DOTween.Kill(transform);
    }

    private void OnDestroy()
    {
        if (_seq != null && _seq.IsActive())
        {
            _seq.Kill();
            _seq = null;
        }
        DOTween.Kill(transform);
    }

    // 외부에서 현재 MoveSpeed로 시퀀스를 재시작하기 위한 공개 메서드
    public void RestartMove()
    {
        StartMoveSequence();
    }

    private void StartMoveSequence()
    {
        if (_seq != null && _seq.IsActive())
        {
            _seq.Kill();
            _seq = null;
        }

        moveDistance = 6f;
        moveTime = (_model != null && _model.MoveSpeed > 0) ? moveDistance / _model.MoveSpeed : 3f;

        float half = moveDistance * 0.5f;

        // 현재 로컬 위치를 12시로 정사각형 중심 계산
        Vector3 start = transform.localPosition; // TopCenter
        Vector3 center = start + Vector3.down * half;

        float z = transform.localPosition.z;
        Vector3 topCenter   = new(center.x,        center.y + half, z); // 시작점(12시)
        Vector3 topRight    = new(center.x + half, center.y + half, z);
        Vector3 bottomRight = new(center.x + half, center.y - half, z);
        Vector3 bottomLeft  = new(center.x - half, center.y - half, z);
        Vector3 topLeft     = new(center.x - half, center.y + half, z);

        float tHalf = half / (_model != null && _model.MoveSpeed > 0 ? _model.MoveSpeed : 2f);
        float tSide = moveDistance / (_model != null && _model.MoveSpeed > 0 ? _model.MoveSpeed : 2f);

        transform.localPosition = topCenter;

        _seq = DOTween.Sequence()
            .SetAutoKill(false);

        _seq.Append(transform.DOLocalMove(topLeft,     tHalf).SetEase(Ease.Linear))
            .AppendCallback(() => _spriteRenderer.flipX = true)
            .Append(transform.DOLocalMove(bottomLeft,  tSide).SetEase(Ease.Linear))   
            .Append(transform.DOLocalMove(bottomRight, tSide).SetEase(Ease.Linear))   
            .AppendCallback(() => _spriteRenderer.flipX = false)
            .Append(transform.DOLocalMove(topRight,    tSide).SetEase(Ease.Linear))   
            .Append(transform.DOLocalMove(topCenter,   tHalf).SetEase(Ease.Linear))   
            .SetLoops(-1, LoopType.Restart);
    }
}