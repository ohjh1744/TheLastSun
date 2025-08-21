using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부위별 아이템 드롭 테이블 클래스
/// 각 레벨(1~10)에 대한 드롭 확률을 저장하고 계산합니다.
/// </summary>
[System.Serializable]
public class DropTable
{
    public string name; // 드롭 테이블 이름 (부위 이름)
    public float[] weights = new float[10]; // 각 레벨(1~10)의 확률 가중치

    public DropTable(string name)
    {
        this.name = name;
    }

    /// <summary>
    /// 정규분포 기반 확률 분포 생성
    /// 플레이어 레벨과 표준편차를 기반으로 가중치 테이블을 생성 (아이템별 드롭 확률(weight) 계산)
    /// </summary>
    /// <param name="characterLevel">현재 캐릭터 레벨 (1~100)</param>
    /// <param name="stdDev">표준편차: 높을수록 분산이 큼</param>
    public void GenerateDistribution(int characterLevel, float stdDev)
    {
        // 평균 레벨 계산: 캐릭터 레벨이 높을수록 평균도 상승 (최대 10)
        // EX> 레벨 50이면 평균(mean) = 6
        //     표준편차 1.5이면 아이템 레벨 = 4~8
        float mean = Mathf.Lerp(2f, 10f, characterLevel / 100f); // 캐릭터 레벨에 따른 평균 아이템 레벨 계산

        float total = 0f;

        // 레벨 1~10에 대해 정규분포 확률 계산
        for (int i = 0; i < 10; i++)
        {
            weights[i] = Gaussian(i + 1, mean, stdDev); // i + 1 == 아이템 레벨
            total += weights[i]; // 전체 합산 (정규화용)
        }

        // 전체 합으로 정규화 => 총합이 1이 되도록 확률화
        for (int i = 0; i < 10; i++)
        {
            weights[i] /= total;
        }
    }

    /// <summary>
    /// 현재 가중치 기반으로 아이템 레벨 선택
    /// 확률 누적 방식 사용
    /// </summary>
    /// <returns>선택된 아이템 레벨 (1~10)</returns>
    public int GetWeightedItemLevel()
    {
        float rand = UnityEngine.Random.value; // 0~1 사이 난수
        float cumulative = 0f; // 누적 확률

        for (int i = 0; i < 10; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative)
                return i + 1; // 실제 레벨은 인덱스 + 1
        }

        return 1; // 오류 방지용 Fallback
    }

    /// <summary>
    /// 정규분포(가우스) 확률 밀도 함수
    /// </summary>
    /// <param name="x">평가할 지점 (아이템 레벨)</param>
    /// <param name="mean">평균 (중심값), 아이템이 가장 자주 나오는 레벨</param>
    /// <param name="stdDev">표준편차, 클수록 다양한 레벨 나옴</param>
    /// <returns>해당 지점의 확률 값, X레벨 드장 확률 빈도 값</returns>
    float Gaussian(float x, float mean, float stdDev) // f(x) = (1 / √(2πσ²)) * exp(-((x - μ)² / (2σ²)))
    {
        float a = 1.0f / (stdDev * Mathf.Sqrt(2.0f * Mathf.PI)); // 정규화 계수 // a = (1 / √(2πσ²))
        float b = Mathf.Exp(-Mathf.Pow(x - mean, 2) / (2.0f * stdDev * stdDev)); // 지수함수 // b = exp(-((x - μ)² / (2σ²)))
        return a * b; // 가우스 함수 결과 // f(x)
    }
}
