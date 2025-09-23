using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGacha : MonoBehaviour
{
    [SerializeField] GameObject[] _units;

    [SerializeField] int[] _probabilitys; // 각 유닛의 등장 확률을 백분율로 설정 (예: 20, 30, 50)

    /// <summary>
    /// Resources/Units 폴더의 모든 프리팹을 _units 배열에 자동으로 할당합니다.
    /// 버튼에 연결하여 사용하세요.
    /// </summary>
    public void LoadUnitsFromResources()
    {
        // Resources/Units 폴더의 모든 GameObject 프리팹 로드
        GameObject[] loadedUnits = Resources.LoadAll<GameObject>("Units");
        if (loadedUnits != null && loadedUnits.Length > 0)
        {
            _units = loadedUnits;
            Debug.Log($"유닛 {loadedUnits.Length}개가 _units 배열에 할당되었습니다.");
        }
        else
        {
            Debug.LogWarning("Resources/Units 폴더에서 유닛 프리팹을 찾지 못했습니다.");
        }
    }

    /// <summary>
    /// _probabilitys에 설정된 확률에 따라 랜덤하게 유닛을 반환합니다.
    /// 예: [20, 30, 50]이면 첫 번째 유닛 20%, 두 번째 30%, 세 번째 50% 확률로 선택됨
    /// </summary>
    public GameObject GetRandomUnit()
    {
        // 1. 확률 합계 계산 (전체 백분율)
        int total = 0;
        for (int i = 0; i < _probabilitys.Length; i++)
            total += _probabilitys[i];

        // 2. 0 ~ total-1 사이의 랜덤 값 생성
        int rand = Random.Range(0, total);

        // 3. 누적 확률로 해당 구간에 속하는 유닛 선택
        int cumulative = 0;
        for (int i = 0; i < _probabilitys.Length; i++)
        {
            cumulative += _probabilitys[i];
            // 랜덤 값이 현재 구간에 속하면 해당 유닛 반환
            if (rand < cumulative)
            {
                return _units[i];
            }
        }

        // 4. 예외 처리: 확률 설정 오류 시 첫 번째 유닛 반환
        return _units.Length > 0 ? _units[0] : null;
    }
}
