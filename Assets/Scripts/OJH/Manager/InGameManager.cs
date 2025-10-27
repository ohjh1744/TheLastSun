using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public enum EPull { Unit, Mob };

public class InGameManager : MonoBehaviour, IAssetLoadable
{
    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }
    #endregion

    #region Addressable Assets
    [SerializeField] private List<AssetReferenceGameObject> _normalUnits;
    [SerializeField] private List<AssetReferenceGameObject> _rareUnits;
    [SerializeField] private List<AssetReferenceGameObject> _ancientUnits;
    [SerializeField] private List<AssetReferenceGameObject> _legendUnits;
    [SerializeField] private List<AssetReferenceGameObject> _epicUnits;
    [SerializeField] private List<AssetReferenceGameObject> _godUnits;

    [SerializeField] private List<AssetReferenceGameObject> _stage1Mobs;
    [SerializeField] private List<AssetReferenceGameObject> _stage2Mobs;
    [SerializeField] private List<AssetReferenceGameObject> _stage3Mobs;
    [SerializeField] private List<AssetReferenceGameObject> _stage4Mobs;
    [SerializeField] private List<AssetReferenceGameObject> _stage5Mobs;
    #endregion IAssetLoadable

    private static InGameManager _instance;
    public static InGameManager Instance { get { return _instance; } set { _instance = value; } }

    private int _jem;
    public int Jem { get { return _jem; } set { _jem = value; } }

    [SerializeField] private ObjectPuller _objectPuller;
    public ObjectPuller ObjectPuller { get { return _objectPuller; } set { _objectPuller = value; } }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadAsset();
    }

    //미리 사전에 유닛별, 필요한 몬스터  미리 프리펩 가져와서 저장
    private void LoadAsset()
    {
        UnitPool unitPool = _objectPuller.UnitPool;

        LoadPrefabs(_normalUnits, unitPool.SavedNormalUnits);
        LoadPrefabs(_rareUnits, unitPool.SavedRareUnits);
        LoadPrefabs(_ancientUnits, unitPool.SavedAncientUnits);
        LoadPrefabs(_legendUnits, unitPool.SavedLegendUnits);
        LoadPrefabs(_epicUnits, unitPool.SavedEpicUnits);
        LoadPrefabs(_godUnits, unitPool.SavedGodUnits);

        //몹 프리펩 가져와서 저장
        PlayerData playerData = PlayerController.Instance.PlayerData;
        MobPool mobPool = _objectPuller.MobPool;

        switch (playerData.CurrentStage)
        {
            case 0:
                LoadPrefabs(_stage1Mobs, mobPool.SavedMobs);
                break;
            case 1:
                LoadPrefabs(_stage2Mobs, mobPool.SavedMobs);
                break;
            case 2:
                LoadPrefabs(_stage3Mobs, mobPool.SavedMobs);
                break;
            case 3:
                LoadPrefabs(_stage4Mobs, mobPool.SavedMobs);
                break;
            case 4:
                LoadPrefabs(_stage5Mobs, mobPool.SavedMobs);
                break;
        }
    }

    private void LoadPrefabs(List<AssetReferenceGameObject> prefabs, List<GameObject> savedPrefabs)
    {
        //프리펩 가져와서 저장
        for (int i = 0; i < prefabs.Count; i++)
        {
            AddressableManager.Instance.GetObjectAndSave(prefabs[i], savedPrefabs, () =>
            {
                _clearLoadAssetCount++;
            });
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Fill _normalUnits from Stage1")]
    private void FillStage1Mob()
    {
        _normalUnits.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage1/Stage1_Mob_{i}.prefab";

            // Addressable에 이미 등록되어 있어야 합니다.
            string addressableKey = AssetDatabase.AssetPathToGUID(assetPath); // GUID를 Key로 사용할 수 있음

            AssetReferenceGameObject reference = new AssetReferenceGameObject(addressableKey);
            _stage1Mobs.Add(reference);
        }

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Fill _normalUnits from Stage2")]
    private void FillStage2Mob()
    {
        _normalUnits.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage2/Stage2_Mob_{i}.prefab";

            // Addressable에 이미 등록되어 있어야 합니다.
            string addressableKey = AssetDatabase.AssetPathToGUID(assetPath); // GUID를 Key로 사용할 수 있음

            AssetReferenceGameObject reference = new AssetReferenceGameObject(addressableKey);
            _stage2Mobs.Add(reference);
        }

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Fill _normalUnits from Stage3")]
    private void FillStage3Mob()
    {
        _normalUnits.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage3/Stage3_Mob_{i}.prefab";

            // Addressable에 이미 등록되어 있어야 합니다.
            string addressableKey = AssetDatabase.AssetPathToGUID(assetPath); // GUID를 Key로 사용할 수 있음

            AssetReferenceGameObject reference = new AssetReferenceGameObject(addressableKey);
            _stage3Mobs.Add(reference);
        }

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Fill _normalUnits from Stage4")]
    private void FillStage4Mob()
    {
        _normalUnits.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage4/Stage4_Mob_{i}.prefab";

            // Addressable에 이미 등록되어 있어야 합니다.
            string addressableKey = AssetDatabase.AssetPathToGUID(assetPath); // GUID를 Key로 사용할 수 있음

            AssetReferenceGameObject reference = new AssetReferenceGameObject(addressableKey);
            _stage4Mobs.Add(reference);
        }

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Fill _normalUnits from Stage5")]
    private void FillStage5Mob()
    {
        _normalUnits.Clear();

        for (int i = 1; i <= 49; i++)
        {
            string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage5/Stage5_Mob_{i}.prefab";

            // Addressable에 이미 등록되어 있어야 합니다.
            string addressableKey = AssetDatabase.AssetPathToGUID(assetPath); // GUID를 Key로 사용할 수 있음

            AssetReferenceGameObject reference = new AssetReferenceGameObject(addressableKey);
            _stage5Mobs.Add(reference);
        }

        EditorUtility.SetDirty(this);
    }
#endif


    //몹소환 관리
    private void SpawnMob()
    {
       
    }


    //게임 시작, 종료 관리


}
