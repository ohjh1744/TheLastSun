using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceLoader : MonoBehaviour
{
    public List<GameObject> myObjects = new List<GameObject>();

    [ContextMenu("Fill List")]
    void FillList()
    {
        myObjects.Clear();

        for (int i = 1; i <= 50; i++)
        {
            string path = $"Prefabs/OJH/Monsters/Stage1/Stage1_Mob_{i}";
            GameObject prefab = Resources.Load<GameObject>(path); // Resources 폴더 안에 있어야 함

            if (prefab != null)
            {
                myObjects.Add(prefab);
            }
            else
            {
                Debug.LogWarning($"프리팹을 찾을 수 없음: {path}");
            }
        }

        Debug.Log($"리스트 채움 완료: {myObjects.Count}개 추가됨");
    }
}
