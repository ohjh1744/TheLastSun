using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private MonsterModel _model => GetComponent<MonsterModel>();
    private PooledObject _pooledObject => GetComponent<PooledObject>();

    private MonsterSpawner _monsterSpawner;

    public Action OnDie;

    private void Awake()
    {
        _monsterSpawner = GetComponentInParent<MonsterSpawner>();
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
        _pooledObject.ReturnPool();
        GameManager.Instance.Jewel += _model.RewardJewel;
        _monsterSpawner._spawnedMonsterCount--;
    }
}
