using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Hardware;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum EMobType {Normal, Boss }
public class MobController : UIBInder, IDamagable
{
    [SerializeField] private MobData _mobData;

    [SerializeField] private TextMeshProUGUI _hpText;

    private int _curHp;
    public int CurHp { get { return _curHp; } set { _curHp = value; } }

    StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
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
        _sb.Append(_curHp);
        GetUI<TextMeshProUGUI>("HpText").SetText(_sb);
    }

    private void Die()
    {
        ObjectPoolManager.Instance.MobNum--;
        transform.DOKill();
        _curHp = _mobData.MaxHp;
    }
}
