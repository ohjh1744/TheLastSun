using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private Vector2[] _spawnPoint;
    private int _curSpawnPoint;

    private Transform _parent;

    // 프리팹별 수량
    public Dictionary<GameObject, int> SpawnedUnitsDic = new();

    // 인스턴스 -> 프리팹 역매핑
    private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

    private void Awake()
    {
        _spawnPoint = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            _spawnPoint[i] = transform.GetChild(i).position;
        }

        if (transform.childCount > 4)
            _parent = transform.GetChild(4);
    }

    public GameObject SpawnUnit(GameObject unitPrefab)
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning("[UnitSpawner] SpawnUnit: 프리팹이 null 입니다.");
            return null;
        }

        Vector3 pos = _spawnPoint[_curSpawnPoint];
        _curSpawnPoint = (_curSpawnPoint >= 3) ? 0 : _curSpawnPoint + 1;

        Transform parent = _parent != null ? _parent : transform;
        GameObject instance = Instantiate(unitPrefab, pos, Quaternion.identity, parent);

        // 카운트 및 매핑 저장
        if (SpawnedUnitsDic.ContainsKey(unitPrefab))
            SpawnedUnitsDic[unitPrefab]++;
        else
            SpawnedUnitsDic[unitPrefab] = 1;

        _instanceToPrefab[instance] = unitPrefab;

        return instance;
    }

    /// <summary>
    /// 버튼에 연결: 지정된 프리팹 유형의 인스턴스 중 1개만 판매(파괴)
    /// </summary>
    public void SellUnit(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[UnitSpawner] SellUnit: 전달된 프리팹이 null");
            return;
        }

        // 해당 프리팹에 해당하는 임의의 인스턴스 1개 찾기
        GameObject targetInstance = null;
        foreach (var kv in _instanceToPrefab)
        {
            if (kv.Value == prefab)
            {
                targetInstance = kv.Key;
                break;
            }
        }

        if (targetInstance == null)
        {
            Debug.LogWarning($"[UnitSpawner] SellUnit: 프리팹 '{prefab.name}' 인스턴스가 존재하지 않습니다.");
            return;
        }

        SellUnitInstance(targetInstance, prefab);

        GetCount(prefab);
    }

    /// <summary>
    /// 내부용: 특정 인스턴스를 직접 판매
    /// </summary>
    public void SellUnitInstance(GameObject instance, GameObject prefab)
    {
        // 수량 감소
        if (SpawnedUnitsDic.TryGetValue(prefab, out int count))
        {
            count--;
            if (count <= 0)
                SpawnedUnitsDic.Remove(prefab);
            else
                SpawnedUnitsDic[prefab] = count;
        }

        _instanceToPrefab.Remove(instance);
        Destroy(instance);
    }

    /// <summary>
    /// 프리팹 현재 수량 조회
    /// </summary>
    public int GetCount(GameObject prefab)
    {
        return prefab != null && SpawnedUnitsDic.TryGetValue(prefab, out int c) ? c : 0;
    }
}
