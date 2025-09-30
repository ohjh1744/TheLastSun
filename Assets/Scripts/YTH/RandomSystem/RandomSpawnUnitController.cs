using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RandomSpawnUnitController : MonoBehaviour
{
    [SerializeField] int _requiredJewl = 12;

    [Header("각 등급별 유닛")]
    [SerializeField] GameObject[] _normalUnits;   // 일반
    [SerializeField] GameObject[] _legendUnits;   // 전설
    [SerializeField] GameObject[] _epicUnits;     // 에픽

    private Vector2[] _spawnPos;
    private int _posIndex = 0;

    public void SpawnRandomUnit()
    {
        GameObject unitPrefab = GetRandomUnit();
        Instantiate(unitPrefab, _spawnPos[_posIndex], Quaternion.identity);
    }

    /// <summary>
    /// 특수소환: 12 태양석을 소모, 일반5/전설3/에픽2 비율로 유닛 가차 
    /// </summary>
    public GameObject GetRandomUnit()
    {
        if (GameManager.Instance.Jewel < _requiredJewl)
        {
            Debug.LogWarning("태양석이 부족합니다.");
            return null;
        }
        GameManager.Instance.Jewel -= _requiredJewl;

        int rand = Random.Range(0, 10);
        GameObject unitPrefab = null;

        if (rand < 5 && rand > 0) // 일반 (0~4)
        {
            unitPrefab = _normalUnits[Random.Range(0, _normalUnits.Length)];
        }
        else if (rand < 8 && rand >= 5) // 전설 (5~7)
        {
            unitPrefab = _legendUnits[Random.Range(0, _legendUnits.Length)];
        }
        else // 에픽 (8~9)
        {
            unitPrefab = _epicUnits[Random.Range(0, _epicUnits.Length)];
        }

        return unitPrefab;
    }
}
