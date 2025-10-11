using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum Class
{
    Normal,
    Rare,
    Ancient,
    Legend,
    Epic,
    God
}

public class RandomSpawnUnitController : MonoBehaviour
{
    [SerializeField] int _requiredJewl = 12;

    [Header("뽑기 확률(%)")]
    [SerializeField] float _normalWeight = 50f;
    [SerializeField] float _rareWeight = 30f;
    [SerializeField] float _ancientWeight = 15f;
    [SerializeField] float _legendWeight = 2f;
    [SerializeField] float _epicWeight = 1.5f;
    [SerializeField] float _godWeight = 0.5f;

    [Header("각 등급별 유닛 (AddressableGameObject)")]
    [SerializeField] AssetReferenceGameObject[] _normalRefs;
    [SerializeField] AssetReferenceGameObject[] _rareRefs;
    [SerializeField] AssetReferenceGameObject[] _ancientRefs;
    [SerializeField] AssetReferenceGameObject[] _legendRefs;
    [SerializeField] AssetReferenceGameObject[] _epicRefs;
    [SerializeField] AssetReferenceGameObject[] _godRefs;

    // 로드된 프리팹 캐시(기존 UI/스포너 호환용)
    private GameObject[] _normalPrefabs;
    private GameObject[] _rarePrefabs;
    private GameObject[] _ancientPrefabs;
    private GameObject[] _legendPrefabs;
    private GameObject[] _epicPrefabs;
    private GameObject[] _godPrefabs;

    // 외부(UI)에서 기존처럼 접근 가능하도록 유지
    public GameObject[] NormalUnits => _normalPrefabs;
    public GameObject[] RareUnits => _rarePrefabs;
    public GameObject[] AncientUnits => _ancientPrefabs;
    public GameObject[] LegendUnits => _legendPrefabs;
    public GameObject[] EpicUnits => _epicPrefabs;
    public GameObject[] GoldUnits => _godPrefabs;

    // 참조별 프리팹 캐시
    private readonly Dictionary<AssetReferenceGameObject, GameObject> _prefabCache = new();

    private UnitSpawner _unitSpawner;

    private void Awake()
    {
        _unitSpawner = GetComponent<UnitSpawner>();

        // 캐시 배열 초기화(인덱스 일치 보장)
        _normalPrefabs = new GameObject[_normalRefs != null ? _normalRefs.Length : 0];
        _rarePrefabs = new GameObject[_rareRefs != null ? _rareRefs.Length : 0];
        _ancientPrefabs = new GameObject[_ancientRefs != null ? _ancientRefs.Length : 0];
        _legendPrefabs = new GameObject[_legendRefs != null ? _legendRefs.Length : 0];
        _epicPrefabs = new GameObject[_epicRefs != null ? _epicRefs.Length : 0];
        _godPrefabs = new GameObject[_godRefs != null ? _godRefs.Length : 0];
    }

    // UI 버튼에 연결할 공개 메서드
    public void SpawnRandomUnit()
    {
        StartCoroutine(SpawnRandomUnitRoutine());
    }

    private IEnumerator SpawnRandomUnitRoutine()
    {
        if (GameManager.Instance.Jewel < _requiredJewl)
        {
            Debug.LogWarning("태양석이 부족합니다.");
            yield break;
        }

        // 배열 & 가중치 매핑 (Addressable refs 기준)
        AssetReferenceGameObject[][] groups =
        {
            _normalRefs,
            _rareRefs,
            _ancientRefs,
            _legendRefs,
            _epicRefs,
            _godRefs
        };

        float[] weights =
        {
            Mathf.Max(0, _normalWeight),
            Mathf.Max(0, _rareWeight),
            Mathf.Max(0, _ancientWeight),
            Mathf.Max(0, _legendWeight),
            Mathf.Max(0, _epicWeight),
            Mathf.Max(0, _godWeight)
        };

        RemoveEmptyWeight(groups, weights);

        float total = 0;
        for (int i = 0; i < weights.Length; i++) total += weights[i];

        if (total <= 0f)
        {
            Debug.LogWarning("소환 가능한 유닛이 없습니다. (가중치/어드레서블 배열 확인)");
            yield break;
        }

        float roll = Random.Range(0, total);
        float acc = 0;
        int pickedIndex = -1;
        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll <= acc)
            {
                pickedIndex = i;
                break;
            }
        }
        if (pickedIndex < 0)
        {
            Debug.LogError("소환 로직 오류: 인덱스 선택 실패");
            yield break;
        }

        var selectedGroup = groups[pickedIndex];
        if (selectedGroup == null || selectedGroup.Length == 0)
        {
            Debug.LogWarning("선택된 그룹이 비어 있습니다.");
            yield break;
        }

        var pickedRef = selectedGroup[Random.Range(0, selectedGroup.Length)];
        if (pickedRef == null || !pickedRef.RuntimeKeyIsValid())
        {
            Debug.LogWarning("유효하지 않은 Addressable 참조입니다.");
            yield break;
        }

        // 프리팹 보장 로드
        GameObject prefab = null;
        yield return EnsurePrefabLoaded(pickedRef, p =>
        {
            prefab = p;
        });

        if (prefab == null)
        {
            Debug.LogError("프리팹 로드 실패로 소환 중단");
            yield break;
        }

        // 성공 시 비용 차감 후 스폰
        GameManager.Instance.Jewel -= _requiredJewl;
        _unitSpawner.SpawnUnit(prefab);
    }

    // Addressable 프리팹을 로드하고 캐시에 저장, 캐시 배열에도 반영
    private IEnumerator EnsurePrefabLoaded(AssetReferenceGameObject aref, System.Action<GameObject> onDone)
    {
        if (_prefabCache.TryGetValue(aref, out var cached) && cached != null)
        {
            onDone?.Invoke(cached);
            yield break;
        }

        var handle = aref.LoadAssetAsync<GameObject>();
        yield return handle;

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogError($"[RandomSpawnUnitController] Addressable 프리팹 로드 실패: {aref.RuntimeKey}");
            onDone?.Invoke(null);
            yield break;
        }

        var prefab = handle.Result;
        _prefabCache[aref] = prefab;

        // 캐시 배열에도 반영(인덱스 일치 전제)
        TryAssignToCacheArray(_normalRefs, _normalPrefabs, aref, prefab);
        TryAssignToCacheArray(_rareRefs, _rarePrefabs, aref, prefab);
        TryAssignToCacheArray(_ancientRefs, _ancientPrefabs, aref, prefab);
        TryAssignToCacheArray(_legendRefs, _legendPrefabs, aref, prefab);
        TryAssignToCacheArray(_epicRefs, _epicPrefabs, aref, prefab);
        TryAssignToCacheArray(_godRefs, _godPrefabs, aref, prefab);

        onDone?.Invoke(prefab);
    }

    private bool TryAssignToCacheArray(AssetReferenceGameObject[] refs, GameObject[] cache, AssetReferenceGameObject key, GameObject prefab)
    {
        if (refs == null || cache == null || refs.Length != cache.Length) return false;
        for (int i = 0; i < refs.Length; i++)
        {
            if (refs[i] == key)
            {
                cache[i] = prefab;
                return true;
            }
        }
        return false;
    }

    private void RemoveEmptyWeight(AssetReferenceGameObject[][] unitGroups, float[] weights)
    {
        for (int i = 0; i < unitGroups.Length; i++)
        {
            var g = unitGroups[i];
            if (g == null || g.Length == 0)
                weights[i] = 0;
        }
    }

    private void OnDestroy()
    {
        // 로드한 에셋 해제
        foreach (var kv in _prefabCache)
        {
            var aref = kv.Key;
            if (aref != null)
                aref.ReleaseAsset();
        }
        _prefabCache.Clear();
    }
}
