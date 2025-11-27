//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public static class CanvasOrderFixer
//{
//    [MenuItem("Tools/Fix Canvas Order For Monsters")]
//    private static void FixCanvasOrder()
//    {
//        // 검색할 폴더 경로들
//        string[] targetFolders = new[]
//        {
//            "Assets/Prefabs/OJH/Monsters/Stage1",
//            "Assets/Prefabs/OJH/Monsters/Stage2",
//            "Assets/Prefabs/OJH/Monsters/Stage3",
//            "Assets/Prefabs/OJH/Monsters/Stage4",
//            "Assets/Prefabs/OJH/Monsters/Stage5"
//        };

//        // 프리팹 검색
//        string[] guids = AssetDatabase.FindAssets("t:Prefab", targetFolders);

//        foreach (var guid in guids)
//        {
//            string path = AssetDatabase.GUIDToAssetPath(guid);
//            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

//            if (prefab == null)
//                continue;

//            var mobControllers = prefab.GetComponentsInChildren<MobController>(true);
//            if (mobControllers.Length == 0)
//                continue;

//            bool changed = false;

//            foreach (var mc in mobControllers)
//            {
//                Canvas canvas = mc.GetComponentInChildren<Canvas>(true);
//                if (canvas == null) continue;

//                if (canvas.sortingOrder != -4)
//                {
//                    canvas.sortingOrder = -4;
//                    changed = true;

//                    Debug.Log($"[변경됨] {prefab.name} → MobController({mc.name}) Canvas sortingOrder = -4");
//                }
//            }

//            if (changed)
//            {
//                PrefabUtility.SavePrefabAsset(prefab);
//                Debug.Log($"[Prefab 저장됨] {prefab.name}");
//            }
//        }

//        AssetDatabase.SaveAssets();
//        Debug.Log("=== 모든 Monster 프리팹 Canvas sortingOrder = -4 적용 완료 ===");
//    }
//}
