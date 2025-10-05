using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private Vector2[] _spawnPoint;
    private int _curSpawnPoint;

    private Transform _parent;

    // 프리팹별 수량
    public Dictionary<GameObject, int> SpawnedUnitsDic = new();


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

        return instance;
    }

    private Vector2 SetSpawnPos()
    {
        Vector2 pos = _spawnPoint[_curSpawnPoint];
        _curSpawnPoint = (_curSpawnPoint >= 3) ? 0 : _curSpawnPoint + 1;

        return pos;
    }

    // 프리팹별 수량 저장
    private void SaveSpawnedUnitsDic(GameObject spawnUnit)
    {
        if (SpawnedUnitsDic.ContainsKey(spawnUnit))
            SpawnedUnitsDic[spawnUnit]++;
        else
            SpawnedUnitsDic[spawnUnit] = 1;
    }
}

