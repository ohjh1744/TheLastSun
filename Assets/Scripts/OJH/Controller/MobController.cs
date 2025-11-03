using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    [SerializeField] private MobData _mobData;

    private int _curHp;
    public int CurHp { get { return _curHp; } set { _curHp = value; } }

    private void OnDisable()
    {
        Die();
    }

    private void DoDamage()
    {

    }

    private void Die()
    {
        ObjectPoolManager.Instance.MobNum--;
        transform.DOKill();
        _curHp = _mobData.MaxHp;
    }
}
