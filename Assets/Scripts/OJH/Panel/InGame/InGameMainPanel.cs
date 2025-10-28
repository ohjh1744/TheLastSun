using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InGameMainPanel : UIBInder, IAssetLoadable
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
    //어드레서블
    [Header("어드레서블 에셋")]
    [SerializeField] private AssetReferenceSprite _buttonSprite;
    [SerializeField] private AssetReferenceSprite _soundOnSprite;
    [SerializeField] private AssetReferenceSprite _soundOffSprite;
    [SerializeField] private AssetReferenceSprite _popUpSprite;
    [SerializeField] private AssetReferenceSprite _jemSprite;
    [SerializeField] private AssetReferenceSprite _barBgSprite;
    [SerializeField] private AssetReferenceSprite _barSprite;
    [SerializeField] private AssetReferenceT<AudioClip>[] _bgmClips;
    [SerializeField] private List<AssetReferenceSprite> _mob1Sprites;
    [SerializeField] private List<AssetReferenceSprite> _mob2Sprites;
    [SerializeField] private List<AssetReferenceSprite> _mob3Sprites;
    [SerializeField] private List<AssetReferenceSprite> _mob4Sprites;
    [SerializeField] private List<AssetReferenceSprite> _mob5Sprites;
    [SerializeField] private AssetReferenceSprite[] _unitSprites;
    #endregion

    #region 저장해서 관리하는 에셋 혹은 UI
    private Sprite _savedSoundOnSprite;
    private Sprite _savedSoundOffSpirte;
    private List<AssetReferenceSprite>[] _savedMobSprites;
    private List<Sprite> _savedCurrentStageMobSprites;
    private List<Color>[] _savedStageMobColors;
    private List<Image> _upgradeButtonPortraitImages;

    [Header("저장해서 사용하기 위한 변수")]
    [SerializeField] private List<Color> _savedStage1MobColors;
    [SerializeField] private List<Color> _savedStage2MobColors;
    [SerializeField] private List<Color> _savedStage3MobColors;
    [SerializeField] private List<Color> _savedStage4MobColors;
    [SerializeField] private List<Color> _savedStage5MobColors;
    #endregion

    [SerializeField] private AudioSource _bgmAudio;

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
        _savedMobSprites = new List<AssetReferenceSprite>[]{_mob1Sprites, _mob2Sprites, _mob3Sprites, _mob4Sprites, _mob5Sprites};

        _savedCurrentStageMobSprites = new List<Sprite>();

        _savedStageMobColors = new List<Color>[]{ _savedStage1MobColors, _savedStage2MobColors, _savedStage3MobColors, _savedStage4MobColors, _savedStage5MobColors};

        _upgradeButtonPortraitImages = new List<Image> { GetUI<Image>("WarriorUpgradeButtonImage"), GetUI<Image>("ArcherUpgradeButtonImage"), GetUI<Image>("BomerUpgradeButtonImage") };

        AddEvent();
        LoadAsset();
    }

    private void AddEvent()
    {

    }

    //UI에 적용할 이미지들 불러오고 적용
    private void LoadAsset()
    {
        AddressableManager.Instance.LoadOnlySprite(_buttonSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("PauseBgImage").sprite = sprite;
            GetUI<Image>("SoundBgImage").sprite = sprite;
            GetUI<Image>("TImeSpeedButton").sprite = sprite;
            GetUI<Image>("WaveInfoPanel").sprite = sprite;
            GetUI<Image>("ShowJemBgImage").sprite = sprite;
            GetUI<Image>("SpawnButtonBgImage").sprite = sprite;
            GetUI<Image>("SpecialSpawnButtonBgImage").sprite = sprite;
            GetUI<Image>("SellButton").sprite = sprite;
            GetUI<Image>("WarriorUpgradeButtonBgImage").sprite = sprite;
            GetUI<Image>("ArcherUpgradeButtonBgImage").sprite = sprite;
            GetUI<Image>("BomerUpgradeButtonBgImage").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_soundOnSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            _savedSoundOnSprite = sprite;
            SetorTurnSound(true);

        });

        AddressableManager.Instance.LoadOnlySprite(_soundOffSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            _savedSoundOffSpirte = sprite;
            SetorTurnSound(true);
        });

        AddressableManager.Instance.LoadOnlySprite(_popUpSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BottomPanel").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_jemSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("ShowJemImage").sprite = sprite;
            GetUI<Image>("SpawnButtonJemImage").sprite = sprite;
            GetUI<Image>("SpecialShowJemButtonJemImage").sprite = sprite;
            GetUI<Image>("ShowWarriorUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowArcherUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowBomerUpgradeInfoJemImage").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_barBgSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("MobNumSliderBg").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_barSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("MobNumSliderFill").sprite = sprite;
        });

        int currentStage = PlayerController.Instance.PlayerData.CurrentStage;
        AddressableManager.Instance.LoadSound(_bgmClips[currentStage], _bgmAudio, () => { _clearLoadAssetCount++; SetorTurnSound(true); });

        for(int i = 0; i < _savedMobSprites[currentStage].Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_savedMobSprites[currentStage][index], (sprite) =>
            {
                _clearLoadAssetCount++;
                _savedCurrentStageMobSprites.Add(sprite);
                if (index == 0)
                {
                    GetUI<Image>("WaveInfoMobImage").sprite = _savedCurrentStageMobSprites[index];
                    GetUI<Image>("WaveInfoMobImage").color = _savedStageMobColors[currentStage][index];
                }
            });
        }

        for(int i = 0; i < _unitSprites.Length; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadSprite(_unitSprites[index], _upgradeButtonPortraitImages[index], () =>
            {
                _clearLoadAssetCount++;
            });
        }

    }

    private void SetorTurnSound(bool isSet)
    {
        var playerData = PlayerController.Instance.PlayerData;
        //변경하는 경우
        if (isSet == false)
        {
            playerData.IsSound = !playerData.IsSound;
        }
        GetUI<Image>("SoundButton").sprite = playerData.IsSound ? _savedSoundOnSprite : _savedSoundOffSpirte;

        //실제 사운드 조정
        if (playerData.IsSound == true)
        {
            _bgmAudio.Play();
        }
        else if (playerData.IsSound == false)
        {
            _bgmAudio.Stop();
        }
    }

    [ContextMenu("FillSPrites")]
    private void FillStageMobSprites()
    {
        _mob2Sprites.Clear();

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
            _mob2Sprites.Add(reference);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"{_mob2Sprites.Count}개의 Stage2 Sprite 참조를 생성했습니다.");
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
            _savedStage5MobColors.Add(color);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"{_savedStage5MobColors.Count}개의 색상을 성공적으로 추가했습니다!");
    }

}
