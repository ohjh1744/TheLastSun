using System.Collections.Generic;
using UnityEngine;
using System; // 이벤트용

public class UnitSpawner : MonoBehaviour
{
    private Vector2[] _spawnPoint;
    private int _curSpawnPoint;

    private Transform _parent;

    // 프리팹별 수량
    public Dictionary<GameObject, int> SpawnedUnitsDic = new();
    public IReadOnlyDictionary<GameObject, int> UnitsCountDic => SpawnedUnitsDic;

    // 프리팹 -> 실제 생성된 인스턴스 목록
    private readonly Dictionary<GameObject, List<GameObject>> _spawnedInstances = new();
    public IReadOnlyDictionary<GameObject, List<GameObject>> SpawnedInstances => _spawnedInstances;

    // 수량 변경 이벤트(UI 갱신용)
    public event Action UnitsCountChanged;

    // 스폰 이벤트(강화 시스템이 구독)
    public event Action<GameObject> UnitSpawned;

    private void Awake()
    {
        _spawnPoint = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            _spawnPoint[i] = transform.GetChild(i).position;
        }

        _parent = transform.GetChild(4);
    }

    public GameObject SpawnUnit(GameObject unitPrefab)
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning("[UnitSpawner] SpawnUnit: 프리팹이 null 입니다.");
            return null;
        }

        GameObject instance = Instantiate(unitPrefab, SetSpawnPos(), Quaternion.identity, _parent);

        SaveSpawnedUnitsDic(unitPrefab);
        SaveSpawnedInstance(unitPrefab, instance);

        UnitSpawned?.Invoke(instance);

        return instance;
    }

    private Vector2 SetSpawnPos()
    {
        Vector2 pos = _spawnPoint[_curSpawnPoint];
        _curSpawnPoint = (_curSpawnPoint >= 3) ? 0 : _curSpawnPoint + 1;

        return pos;
    }

    // 유닛 스폰 시 딕셔너리에 저장(프리팹별 카운트)
    private void SaveSpawnedUnitsDic(GameObject spawnUnit)
    {
        if (SpawnedUnitsDic.ContainsKey(spawnUnit))
            SpawnedUnitsDic[spawnUnit]++;
        else
            SpawnedUnitsDic[spawnUnit] = 1;

        UnitsCountChanged?.Invoke();
    }

    // 유닛 스폰 시 인스턴스 목록에 저장
    private void SaveSpawnedInstance(GameObject prefab, GameObject instance)
    {
        if (!_spawnedInstances.TryGetValue(prefab, out var list))
        {
            list = new List<GameObject>(4);
            _spawnedInstances[prefab] = list;
        }
        list.Add(instance);
    }

    // 유닛 판매 시 수량 감소(프리팹별 카운트)
    public void RemoveSpawnedUnitsDic(GameObject unit)
    {
        if (SpawnedUnitsDic.ContainsKey(unit))
        {
            SpawnedUnitsDic[unit]--;
            if (SpawnedUnitsDic[unit] <= 0)
                SpawnedUnitsDic.Remove(unit);

            UnitsCountChanged?.Invoke();
        }
    }

    // 유닛 판매 함수: 프리팹 기준으로 인스턴스 한 개 파괴
    public void SellUnit(GameObject unitPrefab)
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning("[UnitSpawner] SellUnit: 프리팹이 null 입니다.");
            return;
        }

        if (!_spawnedInstances.TryGetValue(unitPrefab, out var list) || list == null || list.Count == 0)
        {
            Debug.LogWarning($"[UnitSpawner] SellUnit: '{unitPrefab.name}' 인스턴스가 없습니다.");
            return;
        }

        int last = list.Count - 1;
        var instance = list[last];
        list.RemoveAt(last);

        if (instance != null)
        {
            Destroy(instance);
        }

        RemoveSpawnedUnitsDic(unitPrefab);

        if (list.Count == 0)
        {
            _spawnedInstances.Remove(unitPrefab);
        }
    }
}

