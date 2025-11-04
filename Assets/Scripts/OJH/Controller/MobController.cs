using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EMobType {Normal, Boss }
public class MobController : MonoBehaviour
{
    [SerializeField] private MobData _mobData;

    [SerializeField] EMobType _mobType;

    [SerializeField] private Canvas _canvas;

    private int _curHp;
    public int CurHp { get { return _curHp; } set { _curHp = value; } }


    private void Awake()
    {
        if(_mobType == EMobType.Boss)
        {
            _canvas.worldCamera = Camera.main;
        }
    }

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
