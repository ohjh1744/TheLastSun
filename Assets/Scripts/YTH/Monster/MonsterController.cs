using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
   private MonsterModel _model;

    private void Awake()
    {
        _model = GetComponent<MonsterModel>();
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
        Destroy(gameObject);
    }
}
