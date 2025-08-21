using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterModel : MonoBehaviour
{
    [SerializeField] int _maxHp;
    public int MaxHp { get => _maxHp; set => _maxHp = value; }

    [SerializeField] int _curHp;
    public int CurHp { get => _curHp; set => _curHp = value; }

    [SerializeField] int _moveSpeed;
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
}
