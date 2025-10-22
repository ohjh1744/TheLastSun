using DG.Tweening;
using System;
using UnityEngine;

public class MonsterController : MonoBehaviour, IDamageable
{
    private MonsterModel _model => GetComponent<MonsterModel>();
    private PooledObject _pooledObject => GetComponent<PooledObject>();

    private WaveManager _waveManager => WaveManager.Instance;

    public Action OnDie;

    private bool _isDead;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        _isDead = false;
        if (_model != null)
            _model.CurHp = _model.MaxHp;

    }

    public void TakeDamage(int damageAmount)
    {
        if (_isDead || _model == null) return;

        _model.CurHp -= damageAmount;
        if (_model.CurHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return; // 중복 사망 방지
        _isDead = true;

        if (_model != null)
            GameManager.Instance.Jewel += _model.RewardJewel;

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnMonsterDie();

        if (_pooledObject != null)
            _pooledObject.ReturnPool();
        else
            Destroy(gameObject);
    }
}
