using DG.Tweening;
using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private MonsterModel _model => GetComponent<MonsterModel>();
    private PooledObject _pooledObject => GetComponent<PooledObject>();

    private WaveManager _waveManager => WaveManager.Instance;

    public Action OnDie;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        _model.CurHp = _model.MaxHp;
    }

    public void TakeDamage(int damageAmount)
    {
        _model.CurHp -= damageAmount;
        if (_model.CurHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.Jewel += _model.RewardJewel;
       
        WaveManager.Instance.OnMonsterDie();
        transform.localPosition = _waveManager.SpawnPoint.position;

        _pooledObject.ReturnPool();
    }
}
