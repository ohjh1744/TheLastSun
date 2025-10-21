using UnityEngine;


public enum AttakcType
{
    Warrior,
    Archer,
    Bomer
}

public enum Rank
{
    Normal,
    Rare,
    Ancient,
    Legend,
    Epic,
    God
}

[System.Serializable]
public class UnitModel : MonoBehaviour
{
    [SerializeField] Rank _rank;
    public Rank Rank => _rank;

    [Header("공격 관련")]
    [SerializeField] AttakcType _attackType;
    public AttakcType AttackType => _attackType;

   /* [SerializeField] int _attackSpeed;
    public int AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }*/

    [SerializeField] int _attakRange;       // 기본 사거리
    public int AttackRange { get => _attakRange; set => _attakRange = value; }

    [SerializeField] LayerMask _targetLayer = 1 << 6; // monster 레이어
    public LayerMask TargetLayer => _targetLayer;

    [SerializeField] int _damage;
    public int Damage { get => _damage; set => _damage = value; }

    [SerializeField] float _attackDelay;      // 기본 공격 지연(작을수록 빠름)
    public float AttackDelay { get => _attackDelay; set => _attackDelay = value; }


    [Header("강화 관련 - 확인용")]
    [SerializeField] int _level = 1;
    public int Level { get => _level; set => _level = value; }

    [SerializeField] int _add_damage;
    public int Add_Damage { get => _add_damage; set => _add_damage = value; }

    [SerializeField] float _add_attackDelay; // AttackDelay에서 차감되어야함. // 현재 타이머로 구현되어있음
    public float Add_AttackDelay { get => _add_attackDelay; set => _add_attackDelay = Mathf.Max(0, value); }

    // 강화값 일괄 세팅
    public void SetEnhancement()
    {
       Damage += _add_damage;
       AttackDelay -= _add_attackDelay;
    }

    // 강화 초기화
    public void ResetEnhancement(bool resetLevel = false)
    {
        _level = 1;
        _add_damage = 0;
        _add_attackDelay = 0;
    }
}
