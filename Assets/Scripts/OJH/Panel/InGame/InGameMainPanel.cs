using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private AssetReferenceSprite _buttonSprite;
    [SerializeField] private AssetReferenceSprite _soundOnSprite;
    [SerializeField] private AssetReferenceSprite _soundOffSprite;
    [SerializeField] private AssetReferenceSprite _popUpSprite;
    [SerializeField] private AssetReferenceSprite _jemSprite;
    [SerializeField] private AssetReferenceSprite _barBgSprite;
    [SerializeField] private AssetReferenceSprite _barSprite;
    [SerializeField] private AssetReferenceT<AudioClip>[] _bgmClips;
    #endregion

    #region 저장해서 관리하는 에셋 혹은 UI

    private Sprite _savedSoundOnSprite;

    private Sprite _savedSoundOffSpirte;

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
        GetUI();
        AddEvent();
        LoadAsset();
    }

    private void GetUI()
    {
        
    }

    private void AddEvent()
    {
      
    }

    //UI에 적용할 이미지들 불러오고 적용
    private void LoadAsset()
    {
        AddressableManager.Instance.LoadOnlySprite(_buttonSprite, (sprite) => {
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

        AddressableManager.Instance.LoadOnlySprite(_soundOnSprite, (sprite) => {
            _clearLoadAssetCount++;
            _savedSoundOnSprite = sprite;
            SetorTurnSound(true);

        });

        AddressableManager.Instance.LoadOnlySprite(_soundOffSprite, (sprite) => {
            _clearLoadAssetCount++;
            _savedSoundOffSpirte = sprite;
            SetorTurnSound(true);
        });

        AddressableManager.Instance.LoadOnlySprite(_popUpSprite, (sprite) => {
            _clearLoadAssetCount++;
            GetUI<Image>("BottomPanel").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_jemSprite, (sprite) => {
            _clearLoadAssetCount++;
            GetUI<Image>("ShowJemImage").sprite = sprite;
            GetUI<Image>("SpawnButtonJemImage").sprite = sprite;
            GetUI<Image>("SpecialShowJemButtonJemImage").sprite = sprite;
            GetUI<Image>("ShowWarriorUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowArcherUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowBomerUpgradeInfoJemImage").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_barBgSprite, (sprite) => {
            _clearLoadAssetCount++;
            GetUI<Image>("MobNumSliderBg").sprite = sprite;
        });

        AddressableManager.Instance.LoadOnlySprite(_barSprite, (sprite) => {
            _clearLoadAssetCount++;
            GetUI<Image>("MobNumSliderFill").sprite = sprite;
        });

        AddressableManager.Instance.LoadSound(_bgmClips[PlayerController.Instance.PlayerData.CurrentStage], _bgmAudio, () => { _clearLoadAssetCount++; SetorTurnSound(true); });

    }

    private void SetorTurnSound(bool isSet)
    {
        var playerData = PlayerController.Instance.PlayerData;
        //변경하는 경우
        if(isSet == false)
        {
            playerData.IsSound = !playerData.IsSound;
        }
        GetUI<Image>("SoundButton").sprite = playerData.IsSound ? _savedSoundOnSprite : _savedSoundOffSpirte;

        //실제 사운드 조정
        if (playerData.IsSound == true)
        {
            _bgmAudio.Play();
        }
        else if(playerData.IsSound == false)
        {
            _bgmAudio.Stop();
        }
    }



}
