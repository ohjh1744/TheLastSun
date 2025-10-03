using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private MonsterModel _model => GetComponent<MonsterModel>();
    private PooledObject _pooledObject => GetComponent<PooledObject>();

    public Action OnDie;

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
       
        WaveManager.Instance.AliveMonsterCount--;
        _pooledObject.ReturnPool();
    }
}
