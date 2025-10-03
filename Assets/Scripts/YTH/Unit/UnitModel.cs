using UnityEngine;


public enum AttakcType
{
    Melee,
    Ranged
}

[System.Serializable]
public class UnitModel : MonoBehaviour
{
    public int MoveSpeed = 3;

    [Header("공격 관련 ")]

    [SerializeField] AttakcType _attackType;
    public AttakcType AttackType { get => _attackType; }

    [SerializeField] int _attackSpeed;
    public int AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }

    [SerializeField] int _damage;
    public int Damage { get => _damage; set => _damage = value; }

    [SerializeField] int _attakRange;
    public int AttackRange { get => _attakRange; set => _attakRange = value; }

    [SerializeField] int _attackDelay;
    public int AttackDelay { get => _attackDelay; set => _attackDelay = value; }

    private LayerMask _targetLayer = 1 << 6;
    public LayerMask TargetLayer { get => _targetLayer; }
}
