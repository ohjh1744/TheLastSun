using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private MonsterModel _model => GetComponent<MonsterModel>();
    private PooledObject _pooledObject => GetComponent<PooledObject>();

    private WaveManager _monsterSpawner;

    public Action OnDie;

    private void Awake()
    {
        _monsterSpawner = GetComponentInParent<WaveManager>();
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
        // null 로 인해 잠시 주석
       /* _monsterSpawner._aliveMonsterCount--;*/
        _pooledObject.ReturnPool();
    }
}
