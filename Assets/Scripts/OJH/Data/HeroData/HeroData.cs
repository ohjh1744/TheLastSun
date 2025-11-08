using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HeroData")]
public class HeroData : ScriptableObject
{
    [SerializeField] EProjectilePool _heroProjectileIndex;
    public EProjectilePool HeroProjectileIndex { get { return _heroProjectileIndex; } set { _heroProjectileIndex = value; } }

    [SerializeField] private int _baseDamage;
    public int BaseDamage { get { return _baseDamage; } set { _baseDamage = value; } }

    [SerializeField] private float _attackRange;
    public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }

    [SerializeField] private float _attackDelay;
    public float AttackDelay { get { return _attackDelay; } set { _attackDelay = value; } }

    [SerializeField] private AudioClip _spawnClip;
    public AudioClip SpawnClip { get { return _spawnClip; } set { _spawnClip = value; } }

}
