using System.Collections.Generic;
using UnityEngine;

public class UnitEnhancer : MonoBehaviour
{
    public static UnitEnhancer Instance { get; private set; }

    private UnitSpawner _spawner;

    [Header("강화 증가량(레벨당)")]
    [SerializeField] int _damagePerLevel = 1;
    [SerializeField] float _attackDelayReducePerLevel = 0.1f;

    // 클래스(공격 타입)별 강화 레벨
    private readonly Dictionary<AttakcType, int> _typeOfLevelDic = new Dictionary<AttakcType, int>
    {
        { AttakcType.Warrior, 0 },
        { AttakcType.Archer,  0 },
        { AttakcType.Bomer,   0 }
    };

    private void Start()
    {
        if (_spawner == null) _spawner = GetComponent<UnitSpawner>();
        if (_spawner == null) _spawner = FindObjectOfType<UnitSpawner>();
    }

    private void OnEnable()
    {
        if (_spawner == null)
        {
            // 씬 로딩 순서에 따라 늦게 생길 수 있으니 한 번 더 시도
            _spawner = GetComponent<UnitSpawner>() ?? FindObjectOfType<UnitSpawner>();
        }

        _spawner.UnitSpawned += OnUnitSpawned;
    }

    private void OnDisable()
    {

        _spawner.UnitSpawned -= OnUnitSpawned;
    }

    public void EnhanceArcher() => AddLevel(AttakcType.Archer);
    public void EnhanceBomer() => AddLevel(AttakcType.Bomer);
    public void EnhanceWarrior() => AddLevel(AttakcType.Warrior);

    private void OnUnitSpawned(GameObject instance)
    {
        UnitModel model = instance.GetComponent<UnitModel>();

        ApplyEnhancement(model);
    }



    // 레벨 증가 + 씬 내 기존 유닛 모두 재적용
    public void AddLevel(AttakcType attackType)
    {
        _typeOfLevelDic[attackType] = Mathf.Max(0, _typeOfLevelDic[attackType] + 1);
        ApplyAllExistingOfType(attackType);
    }

    private void ApplyAllExistingOfType(AttakcType attackType)
    {
        foreach (var item in _spawner.SpawnedInstances)
        {
            List<GameObject> list = item.Value;
            if (list == null) continue;

            foreach (GameObject unit in list)
            {
                if (unit == null) continue;
                UnitModel model = unit.GetComponent<UnitModel>();
                if (model == null || model.AttackType != attackType) continue;

                ApplyEnhancement(model);
            }
        }
    }

    // 원본 스텟 기준으로 현재 레벨을 반영하여 재계산
    private void ApplyEnhancement(UnitModel model)
    {
        int level = _typeOfLevelDic[model.AttackType];

        model.Damage += _damagePerLevel;
        model.AttackDelay = Mathf.Max(0, model.AttackDelay - _attackDelayReducePerLevel);


        // 인스펙터상 강화 수치 확인용
        model.Add_Damage = _damagePerLevel * level;
        model.Add_AttackDelay = Mathf.Max(0, _attackDelayReducePerLevel * level);
        Debug.Log($"Applied Enhancement: {model.AttackType} Level {level} -> Damage +{model.Add_Damage}, AttackDelay -{model.Add_AttackDelay}");
     /*   model.SetEnhancement();*/
    }
}
