using UnityEngine;


[System.Serializable]
public class UnitModel : MonoBehaviour
{
    public int MoveSpeed = 3;

    [Header("공격 관련 ")]

    [SerializeField] int _attackSpeed;
    public int AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }

    [SerializeField] int _damage;
    public int Damage { get => _damage; set => _damage = value; }

    [SerializeField] int _attakRange;
    public int AttackRange { get => _attakRange; set => _attakRange = value; }

    [SerializeField] int _attackDelay;
    public int AttackDelay { get => _attackDelay; set => _attackDelay = value; }

    [SerializeField] LayerMask _targetLayer => LayerMask.GetMask("Monster");
    public LayerMask TargetLayer { get => _targetLayer; }
}
