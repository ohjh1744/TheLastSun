using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum EMobType {Normal, Boss }
public class MobController : UIBInder, IDamagable
{
    [SerializeField] private MobData _mobData;

    public MobData MobData { get { return _mobData; } private set { } }

    [SerializeField] private TextMeshProUGUI _hpText;

    private int _curHp;
    public int CurHp { get { return _curHp; } set { _curHp = value; } }

    StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        _curHp = _mobData.MaxHp;
        _sb.Clear();
        _sb.Append(_curHp);
        GetUI<TextMeshProUGUI>("HpText").SetText(_sb);
    }


    private void OnDisable()
    {
        Die();
    }

    public void TakeDamage(int damage)
    {
        _curHp -= damage;
        if (_curHp <= 0)
        {
            _curHp = 0;
            gameObject.SetActive(false);
        }
        _sb.Clear();
        _sb.Append(_curHp);
        GetUI<TextMeshProUGUI>("HpText").SetText(_sb);
    }

    private void Die()
    {
        InGameManager.Instance.JemNum += _mobData.JemForKill;
        ObjectPoolManager.Instance.MobNum--;
        transform.DOKill();
    }
}
