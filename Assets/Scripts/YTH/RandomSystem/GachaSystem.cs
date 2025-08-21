using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GachaSystem : MonoBehaviour
{
    [Header("캐릭터 레벨 설정 (1~100)")]
    [Range(1, 100)]
    public int characterLevel = 1;  // 플레이어의 현재 레벨 (확률 분포 중심값에 영향)

    [Header("표준편차: 클수록 레벨 분산 커짐")]
    public float stdDev = 2.0f;  // 정규분포의 표준편차, 높을수록 더 다양한 레벨 등장

    [Header("아이템 부위별 커스텀 확률 테이블")]
    public DropTable helmetTable = new DropTable("Helmet"); // 헬멧 아이템 드롭 테이블
    public DropTable armorTable = new DropTable("Armor");   // 갑옷 아이템 드롭 테이블
    public DropTable weaponTable = new DropTable("Weapon"); // 무기 아이템 드롭 테이블
    public DropTable cloakTable = new DropTable("Cloak");   // 망토 아이템 드롭 테이블

    [Header("천장 시스템: 실패 누적 시 보상")]
    public int pityThreshold = 10; // 낮은 레벨 아이템이 연속으로 나올 수 있는 최대 횟수
    private int pityCounter = 0;   // 현재까지 낮은 레벨이 나온 횟수

    // 아이템 부위 타입 정의
    public enum ItemType { Helmet, Armor, Weapon, Cloak }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetRandomItem(); // ItemType과 Level을 받아 그 아이템을 생성하여 길가에다가 Instantiate하는게 좋을듯
        }
    }

    /// <summary>
    /// 에디터 상에서 캐릭터 레벨을 변경할 때 자동으로 드롭 테이블 갱신
    /// </summary>
    void OnValidate()
    {
        helmetTable.GenerateDistribution(characterLevel, stdDev);
        armorTable.GenerateDistribution(characterLevel, stdDev);
        weaponTable.GenerateDistribution(characterLevel, stdDev);
        cloakTable.GenerateDistribution(characterLevel, stdDev);
    }

    /// <summary>
    /// 실제 뽑기 실행 함수
    /// 부위 랜덤 선택 → 레벨 확률 계산 → 천장 시스템 적용 → 결과 반환
    /// </summary>
    /// <returns>(부위, 아이템 레벨) 튜플 반환</returns>
    public (ItemType itemType, int level) GetRandomItem()
    {
        // 0~3 사이 랜덤 정수로 부위 선택
        ItemType type = (ItemType)UnityEngine.Random.Range(0, 4);

        int level = 1; // 기본 아이템 레벨
        DropTable table = GetTable(type); // 해당 부위의 드롭 테이블 가져오기

        // 천장 시스템: 연속으로 낮은 레벨이 나오면 최고 레벨 지급
        if (pityCounter >= pityThreshold)
        {
            level = 10;          // 최고 레벨 아이템 보상
            pityCounter = 0;     // pity 초기화
        }
        else
        {
            // 확률 기반 레벨 뽑기
            level = table.GetWeightedItemLevel();

            // 높은 레벨(9 이상)이면 pity 초기화, 아니면 누적
            pityCounter = level >= 9 ? 0 : pityCounter + 1;
        }

        // 디버그 로그 출력
        Debug.Log($"[{type}] Lv.{level} 아이템 획득");

        return (type, level); // 결과 반환
    }

    /// <summary>
    /// 아이템 타입에 따른 해당 드롭 테이블 반환
    /// </summary>
    /// <param name="type">아이템 부위 타입</param>
    /// <returns>해당 부위의 드롭 테이블</returns>
    DropTable GetTable(ItemType type)
    {
        return type switch
        {
            ItemType.Helmet => helmetTable,
            ItemType.Armor => armorTable,
            ItemType.Weapon => weaponTable,
            ItemType.Cloak => cloakTable,
            _ => helmetTable // 기본값은 헬멧
        };
    }
}
