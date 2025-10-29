using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InGameMainPanel : UIBInder, IAssetLoadable
{
    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        AddEvent();
    }

    private void AddEvent()
    {

    }

    //UI에 적용할 이미지들 불러오고 적용
  
   
    [ContextMenu("FillSPrites")]
    private void FillStageMobSprites()
    {
        //_mob2Sprites.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage2/Stage2_Mob_{i}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (!prefab)
            {
                Debug.LogWarning($"프리팹을 찾을 수 없습니다: {assetPath}");
                continue;
            }

            // SpriteRenderer에서 Sprite 가져오기
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite)
            {
                Debug.LogWarning($"{prefab.name}에 SpriteRenderer 또는 Sprite가 없습니다.");
                continue;
            }

            // Sprite의 GUID를 가져와 AssetReferenceSprite 생성
            string spritePath = AssetDatabase.GetAssetPath(sr.sprite);
            string guid = AssetDatabase.AssetPathToGUID(spritePath);

            AssetReferenceSprite reference = new AssetReferenceSprite(guid);
            //_mob2Sprites.Add(reference);
        }

        EditorUtility.SetDirty(this);
        //Debug.Log($"{_mob2Sprites.Count}개의 Stage2 Sprite 참조를 생성했습니다.");
    }

    [ContextMenu("Fill Colors")]
    private void FillStageMobColors()
    {
        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage5/Stage5_Mob_{i}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab == null)
            {
                Debug.LogWarning($"프리팹을 찾을 수 없습니다: {assetPath}");
                continue;
            }

            // SpriteRenderer에서 색상 가져오기
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning($"{prefab.name}에서 SpriteRenderer를 찾을 수 없습니다.");
                continue;
            }

            Color color = sr.color;
           // _savedStage5MobColors.Add(color);
        }

        EditorUtility.SetDirty(this);
       // Debug.Log($"{_savedStage5MobColors.Count}개의 색상을 성공적으로 추가했습니다!");
    }

}
