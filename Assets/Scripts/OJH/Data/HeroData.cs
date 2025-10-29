using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HeroData")]
public class HeroData : ScriptableObject
{
    [SerializeField] private int _baseDamage;
    public int BaseDamage { get { return _baseDamage; } set { _baseDamage = value; } }

    [SerializeField] private int _attackRange;
    public int AttackRange { get { return _attackRange; } set { _attackRange = value; } }

    [SerializeField] private int _attackDelay;
    public int AttackDelay { get { return _attackDelay; } set { _attackDelay = value; } }
}
