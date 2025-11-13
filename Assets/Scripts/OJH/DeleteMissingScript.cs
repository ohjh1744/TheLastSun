//using UnityEngine;
//using UnityEditor;

//public class DeleteMissingScript : Editor
//{
//    //[MenuItem("Tools/Remove Missing Scripts Recursively")]
//    //public static void RemoveMissingScriptsa()
//    //{
//    //    // 현재 씬의 모든 GameObject를 대상으로 합니다.
//    //    foreach (GameObject go in GetAllObjectsInScene(true))
//    //    {
//    //        RemoveMissingScriptsFromGameObject(go);
//    //    }
//    //}

//    //static void RemoveMissingScriptsFromGameObject(GameObject go)
//    //{
//    //    // 누락된 스크립트 제거
//    //    int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
//    //    if (removedCount > 0)
//    //    {
//    //        Debug.Log($"{removedCount} missing scripts removed from {go.name}", go);
//    //    }

//    //    // 자식 GameObject들에 대해서도 재귀적으로 처리
//    //    foreach (Transform child in go.transform)
//    //    {
//    //        RemoveMissingScriptsFromGameObject(child.gameObject);
//    //    }
//    //}

//    //static GameObject[] GetAllObjectsInScene(bool includeInactive)
//    //{
//    //    return GameObject.FindObjectsOfType<GameObject>(includeInactive);
//    //}
//}