using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "UnitModelSO/UnitData", order = 1)]
public class UnitModelSO : ScriptableObject
{
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

        [Header("강화 관련")]
        public int add_damage;
    }
}
