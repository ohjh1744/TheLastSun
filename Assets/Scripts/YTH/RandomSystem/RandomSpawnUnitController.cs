using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RandomSpawnUnitController : MonoBehaviour
{
    [SerializeField] int _requiredJewl = 12;

    [Header("뽑기 확률 (가중치)")]
    [SerializeField] int _tier1 = 1;    // 신화 
    [SerializeField] int _tier2 = 4;    // 에픽
    [SerializeField] int _tier3 = 25;   // 레어 
    [SerializeField] int _tIer4 = 70;   // 일반 (변수명 오타 주의: _tier4 아님)

    [Header("각 등급별 유닛")]
    [SerializeField] GameObject[] _tier1_Units;   // 신화
    [SerializeField] GameObject[] _tier2_Units;   // 에픽
    [SerializeField] GameObject[] _tier3_Units;   // 레어
    [SerializeField] GameObject[] _tier4_Units;   // 일반

    private UnitSpawner _unitSpawner;

    private void Awake()
    {
        _unitSpawner = GetComponent<UnitSpawner>();
    }

    public void SpawnRandomUnit()
    {
        GameObject unitPrefab = GetRandomUnit();
        if (unitPrefab != null)
        {
            _unitSpawner.SpawnUnit(unitPrefab);
        }
    }

    /// <summary>
    /// 특수소환: 비용(_requiredJewl) 소모 후 확률 가중치 기반 소환
    /// </summary>
    public GameObject GetRandomUnit()
    {
        if (GameManager.Instance.Jewel < _requiredJewl)
        {
            Debug.LogWarning("태양석이 부족합니다.");
            return null;
        }

        // 가중치 (음수 방지)
        int w1 = Mathf.Max(0, _tier1);
        int w2 = Mathf.Max(0, _tier2);
        int w3 = Mathf.Max(0, _tier3);
        int w4 = Mathf.Max(0, _tIer4);

        // 비어있는 등급 제거
        if (_tier1_Units == null || _tier1_Units.Length == 0) w1 = 0;
        if (_tier2_Units == null || _tier2_Units.Length == 0) w2 = 0;
        if (_tier3_Units == null || _tier3_Units.Length == 0) w3 = 0;
        if (_tier4_Units == null || _tier4_Units.Length == 0) w4 = 0;

        int total = w1 + w2 + w3 + w4;

        int prob = Random.Range(0, total);
        GameObject spawnUnit = null;

        if (prob < w1)
        {
            spawnUnit = _tier1_Units[Random.Range(0, _tier1_Units.Length)];
        }
        else if (prob < w1 + w2)
        {
            spawnUnit = _tier2_Units[Random.Range(0, _tier2_Units.Length)];
        }
        else if (prob < w1 + w2 + w3)
        {
            spawnUnit = _tier3_Units[Random.Range(0, _tier3_Units.Length)];
        }
        else
        {
            spawnUnit = _tier4_Units[Random.Range(0, _tier4_Units.Length)];
        }

        GameManager.Instance.Jewel -= _requiredJewl;
        return spawnUnit;
    }
}
