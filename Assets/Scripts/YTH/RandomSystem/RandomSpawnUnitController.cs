using UnityEngine;

public enum Class
{
    Normal,  
    Rare,    
    Ancient, 
    Legend,
    Epic,
    Gold     
}

public class RandomSpawnUnitController : MonoBehaviour
{
    [SerializeField] int _requiredJewl = 12;

    [Header("뽑기 확률")]
    [SerializeField] float _normalWeight = 50f;    
    [SerializeField] float _rareWeight = 30f;      
    [SerializeField] float _ancientWeight = 15f;   
    [SerializeField] float _legendWeight = 2f;     
    [SerializeField] float _epicWeight = 1.5f;     
    [SerializeField] float _goldWeight = 0.5f;     

    [Header("각 등급별 유닛 (UnitTier 순서대로)")]
    [SerializeField] GameObject[] _normalUnits;
    [SerializeField] GameObject[] _rareUnits;
    [SerializeField] GameObject[] _ancientUnits;
    [SerializeField] GameObject[] _legendUnits;
    [SerializeField] GameObject[] _epicUnits;
    [SerializeField] GameObject[] _goldUnits;

    private UnitSpawner _unitSpawner;

    private void Awake()
    {
        _unitSpawner = GetComponent<UnitSpawner>();
    }

    public void SpawnRandomUnit()
    {
        var unitPrefab = GetRandomUnit();
        if (unitPrefab != null)
        {
            _unitSpawner.SpawnUnit(unitPrefab);
        }
    }

    public GameObject GetRandomUnit()
    {
        if (GameManager.Instance.Jewel < _requiredJewl)
        {
            Debug.LogWarning("태양석이 부족합니다.");
            return null;
        }

        // 배열 & 가중치 매핑
        GameObject[][] unitGroups =
        {
            _normalUnits,
            _rareUnits,
            _ancientUnits,
            _legendUnits,
            _epicUnits,
            _goldUnits
        };

        float[] weights =
        {
            Mathf.Max(0, _normalWeight),
            Mathf.Max(0, _rareWeight),
            Mathf.Max(0, _ancientWeight),
            Mathf.Max(0, _legendWeight),
            Mathf.Max(0, _epicWeight),
            Mathf.Max(0, _goldWeight)
        };

        RemoveEmptyWeight(unitGroups, weights);

        float total = 0;
        for (int i = 0; i < weights.Length; i++)
            total += weights[i];

        if (total == 0)
        {
            Debug.LogWarning("소환 가능한 유닛이 없습니다. (가중치/배열 확인)");
            return null;
        }

        int roll = (int)Random.Range(0, total);
        float acc = 0;
        int pickedIndex = -1;

        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll < acc)
            {
                pickedIndex = i;
                break;
            }
        }

        if (pickedIndex == -1)
        {
            Debug.LogError("소환 로직 오류: 인덱스 선택 실패");
            return null;
        }

        GameObject[] selectedGroup = unitGroups[pickedIndex];
        GameObject selected = selectedGroup[Random.Range(0, selectedGroup.Length)];

        GameManager.Instance.Jewel -= _requiredJewl;
        return selected;
    }

    private void RemoveEmptyWeight(GameObject[][] unitGroups, float[] weights)
    {
          // 비어있는 그룹은 가중치 제거
        for (int i = 0; i < unitGroups.Length; i++)
        {
            if (unitGroups[i] == null || unitGroups[i].Length == 0)
                weights[i] = 0;
        }
    }

    private void GetWeight()
    {

    }
}
