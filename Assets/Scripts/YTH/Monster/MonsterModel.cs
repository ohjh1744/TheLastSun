using System;
using UnityEngine;

[Serializable]
public class MonsterModel : MonoBehaviour
{
    [SerializeField] bool _isBoss;
    public bool IsBoss { get => _isBoss; set => _isBoss = value; }

    private int _rewardJewel;
    public int RewardJewel { get => IsBoss ? 12 : 1; }

    [SerializeField] int _maxHp;
    public int MaxHp { get => _maxHp; set => _maxHp = value; }

    [SerializeField] int _curHp;
    public int CurHp { get => _curHp; set => _curHp = value; }

    private int _moveSpeed;
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
}
