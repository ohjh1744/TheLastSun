using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// 커스텀 인스펙터 클래스: GachaSystem 스크립트에 대한 에디터 시각화
/// 아이템 드롭 확률 분포를 시각적으로 확인할 수 있도록 지원
/// </summary>
[CustomEditor(typeof(GachaSystem))]
public class GachaSystemEditor : Editor
{
    /// <summary>
    /// 인스펙터 GUI 커스터마이징
    /// </summary>
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        // 대상 GachaSystem 인스턴스 참조
        GachaSystem gacha = (GachaSystem)target;

        // 분포 시각화 제목 라벨
        GUILayout.Label("\n아이템 레벨 분포 (시각화)", EditorStyles.boldLabel);

        // 각 부위별 드롭 테이블 시각화
        DrawDistribution("Helmet", gacha.helmetTable);
        DrawDistribution("Armor", gacha.armorTable);
        DrawDistribution("Weapon", gacha.weaponTable);
        DrawDistribution("Cloak", gacha.cloakTable);
    }

    /// <summary>
    /// 한 부위의 레벨 분포를 막대 그래프로 시각화
    /// </summary>
    /// <param name="label">부위 이름 (예: Helmet)</param>
    /// <param name="table">해당 부위의 드롭 테이블</param>
    void DrawDistribution(string label, DropTable table)
    {
        // 부위 라벨 출력
        GUILayout.Label($"[{label}]");

        EditorGUILayout.BeginHorizontal(); // 가로 방향으로 막대 나열
        for (int i = 0; i < 10; i++)
        {
            float height = table.weights[i] * 200f; // 높이 = 확률 * 스케일
            Rect rect = GUILayoutUtility.GetRect(10, height); // 막대 영역 설정

            // 색상: 낮은 레벨은 빨강 → 높은 레벨은 초록으로 점진적 변경
            Color color = Color.Lerp(Color.red, Color.green, i / 9f);
            EditorGUI.DrawRect(rect, color); // 막대 그리기
        }
        EditorGUILayout.EndHorizontal(); // 가로 레이아웃 종료
    }
}
#endif
