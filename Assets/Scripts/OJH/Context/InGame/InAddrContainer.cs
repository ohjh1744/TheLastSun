using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InAddrContainer : UIBInder, IAssetLoadable
{
    private enum EHero {Normal, Rare, Ancient, Legend, Epic}
    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }
    #endregion

    #region Addressable Assets
    //UI
    [Header("UI")]
    [SerializeField] AssetReferenceSprite _button_03;
    [SerializeField] AssetReferenceSprite _button_04;
    [SerializeField] AssetReferenceSprite _cancel;
    [SerializeField] AssetReferenceSprite _diamond;
    [SerializeField] AssetReferenceSprite _icon_Volume;
    [SerializeField] AssetReferenceSprite _icon_VolumeMute;
    [SerializeField] AssetReferenceSprite _pause;
    [SerializeField] AssetReferenceSprite _popUp;
    [SerializeField] AssetReferenceSprite _popUp_Title;
    [SerializeField] AssetReferenceSprite _resourceBar1;
    [SerializeField] AssetReferenceSprite _resourceBar2;
    [SerializeField] List<AssetReferenceSprite> _backgroundSprites;
    [SerializeField] List<AssetReferenceSprite> _tutorialSprites;
    [SerializeField] List<AssetReferenceSprite> _wave1MobSprites;

    //추후 넣을 것
    [SerializeField] AssetReferenceSprite _spawnButton;
    [SerializeField] AssetReferenceSprite _specialSpawnButton;
    [SerializeField] List<AssetReferenceSprite> _warriorPortraitSprites;
    [SerializeField] List<AssetReferenceSprite> _archerPortraitSprites;
    [SerializeField] List<AssetReferenceSprite> _bomerPortraitSprites;
    [SerializeField] List<AssetReferenceSprite> _godPortraitSprites;

    //Prefab
    [Header("Hero")]
    [SerializeField] List<AssetReferenceGameObject> _heros;

    [Header("Mob")]
    [SerializeField] List<AssetReferenceGameObject> _stage1Mobs;
    [SerializeField] List<AssetReferenceGameObject> _stage2Mobs;
    [SerializeField] List<AssetReferenceGameObject> _stage3Mobs;
    [SerializeField] List<AssetReferenceGameObject> _stage4Mobs;
    [SerializeField] List<AssetReferenceGameObject> _stage5Mobs;

    [Header("Projectile")]
    [SerializeField] List<AssetReferenceGameObject> _projectiles;

    [Header("HeroPlate")]
    [SerializeField] AssetReferenceGameObject _heroPlate;

    //SOund
    [Header("Sound")]
    [SerializeField] List<AssetReferenceT<AudioClip>> _bgmClip;
    [SerializeField] AssetReferenceT<AudioClip> _spawnClip;
    #endregion

    [SerializeField] private GameObject _addressObjects;

    void Awake()
    {
        BindAll();
    }

    void Start()
    {
        LoadAsset();
    }

    void LoadAsset()
    {
        AddressableManager.Instance.LoadOnlySprite(_button_03, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("GoSellButton").sprite = sprite;
            GetUI<Image>("ClearPanelGoLobbyButton").sprite = sprite;
            GetUI<Image>("GoTutorialButton").sprite = sprite;
            GetUI<Image>("PausePanelGoLobbyButton").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_button_04, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("PauseBgImage").sprite = sprite;
            GetUI<Image>("SoundBgImage").sprite = sprite;
            GetUI<Image>("TImeSpeedButton").sprite = sprite;
            GetUI<Image>("WaveInfoPanel").sprite = sprite;
            GetUI<Image>("ShowJemBgImage").sprite = sprite;
            GetUI<Image>("ClearPanelSetFalseBgImage").sprite = sprite;
            GetUI<Image>("PausePanelSetFalseBgImage").sprite = sprite;
            GetUI<Image>("SellPanelSetFalseButtonBgImage").sprite = sprite;

        });
        AddressableManager.Instance.LoadOnlySprite(_cancel, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("ClearPanelSetFalseButton").sprite = sprite;
            GetUI<Image>("PausePanelSetFalseButton").sprite = sprite;
            GetUI<Image>("SellPanelSetFalseButton").sprite = sprite;

        });
        AddressableManager.Instance.LoadOnlySprite(_diamond, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("ShowJemImage").sprite = sprite;
            GetUI<Image>("ShowJemImage").sprite = sprite;
            GetUI<Image>("ShowWarriorUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowArcherUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("ShowBomerUpgradeInfoJemImage").sprite = sprite;
            GetUI<Image>("GetNormalJemImage").sprite = sprite;
            GetUI<Image>("GetRareJemImage").sprite = sprite;
            GetUI<Image>("GetAncientJemImage").sprite = sprite;
            GetUI<Image>("GetLegendJemImage").sprite = sprite;
            GetUI<Image>("GetEpicJemImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_icon_Volume, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("SoundButton").sprite = sprite; if (PlayerController.Instance.PlayerData.IsSound) { GetUI("SoundButton").SetActive(true); } });
        AddressableManager.Instance.LoadOnlySprite(_icon_VolumeMute, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("SoundMuteButton").sprite = sprite; if (!PlayerController.Instance.PlayerData.IsSound) { GetUI("SoundMuteButton").SetActive(true); } });
        AddressableManager.Instance.LoadOnlySprite(_pause, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("PauseButton").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_popUp, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BottomPanel").sprite = sprite;
            GetUI<Image>("ClearPanel").sprite = sprite;
            GetUI<Image>("PausePanel").sprite = sprite;
            GetUI<Image>("SellPanel").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_popUp_Title, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("ClearPanelNameBgImage").sprite = sprite;
            GetUI<Image>("PausePanelNameBgImage").sprite = sprite;
            GetUI<Image>("SellPanelNameBgImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_resourceBar1, (sprite) => { 
            _clearLoadAssetCount++;
            GetUI<Image>("ShowJemBgImage").sprite = sprite;
            GetUI<Image>("MobNumSliderBg").sprite = sprite;
            GetUI<Image>("NotifyPanel").sprite = sprite;
            GetUI<Image>("ChangeSellWarriorButton").sprite = sprite;
            GetUI<Image>("ChangeSellArcherButton").sprite = sprite;
            GetUI<Image>("ChangeSellBomerButton").sprite = sprite;
            GetUI<Image>("ShowWarriorUpgradeInfoBgImage").sprite = sprite;
            GetUI<Image>("ShowArcherUpgradeInfoBgImage ").sprite = sprite;
            GetUI<Image>("ShowBomerUpgradeInfoBgImage").sprite = sprite;

            GetUI<Image>("GetNormalJemInfoBgImage").sprite = sprite;
            GetUI<Image>("GetRareJemInfoBgImage").sprite = sprite;
            GetUI<Image>("GetAncientJemInfoBgImage").sprite = sprite;
            GetUI<Image>("GetLegendJemInfoBgImage").sprite = sprite;
            GetUI<Image>("GetEpicJemInfoBgImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_resourceBar2, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("MobNumSliderFill").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_backgroundSprites[PlayerController.Instance.PlayerData.CurrentStage], (sprite) =>{_clearLoadAssetCount++;GetUI<Image>("InGameBgPanel").sprite= sprite;});
        for (int i = 0; i < _tutorialSprites.Count; i++)
        {
            _clearLoadAssetCount++;
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_tutorialSprites[index], (sprite) => {
                GetUI<Image>($"TutorialImage{index}").sprite = sprite;
            });
        }
        AddressableManager.Instance.LoadOnlySprite(_wave1MobSprites[PlayerController.Instance.PlayerData.CurrentStage], (sprite) => { _clearLoadAssetCount++; GetUI<Image>("WaveInfoMobImage").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_spawnButton, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("SpawnButton").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_specialSpawnButton, (sprite) => { 
            _clearLoadAssetCount++; 
            GetUI<Image>("SpecialSpawnButton").sprite = sprite;
            GetUI<Image>("SpecialSpawnButton").sprite = sprite;
        });

        for(int i = 0; i < _warriorPortraitSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_warriorPortraitSprites[index], (sprite) =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>($"{((EHero)index).ToString()}WarriorSellButton").sprite = sprite;
                if(index == (int)EHero.Normal)
                {
                    GetUI<Image>("WarriorUpgradeButton").sprite = sprite;
                }
            });
        }
        for (int i = 0; i < _archerPortraitSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_archerPortraitSprites[index], (sprite) =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>($"{((EHero)index).ToString()}ArcherSellButton").sprite = sprite;
                if (index == (int)EHero.Normal)
                {
                    GetUI<Image>("ArcherUpgradeButton").sprite = sprite;
                }
            });
        }
        for (int i = 0; i < _bomerPortraitSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_bomerPortraitSprites[index], (sprite) =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>($"{((EHero)index).ToString()}BomerSellButton").sprite = sprite;
                if (index == (int)EHero.Normal)
                {
                    GetUI<Image>("BomerUpgradeButton").sprite = sprite;
                }
            });
        }

        AddressableManager.Instance.LoadSound(_bgmClip[PlayerController.Instance.PlayerData.CurrentStage], GetUI<AudioSource>("Bgm"), () => {
            _clearLoadAssetCount++;
            if (PlayerController.Instance.PlayerData.IsSound == true)
            {
                GetUI<AudioSource>("Bgm").Play();
            }
        });
        AddressableManager.Instance.LoadSound(_spawnClip, GetUI<AudioSource>("Sfx"), () => { _clearLoadAssetCount++;});

        //아래부터는 프리펩 불러오고 미리 생성
        for(int i = 0; i < _heros.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.GetObjectAndSave(_heros[index], ObjectPoolManager.Instance.Heros, (obj) =>{ _clearLoadAssetCount++; obj.transform.SetParent(_addressObjects.transform, worldPositionStays: false);});
        }

        LoadMob();

        for (int i = 0; i < _projectiles.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.GetObjectAndSave(_projectiles[index], ObjectPoolManager.Instance.Projectiles,  (obj) =>{ _clearLoadAssetCount++; obj.transform.SetParent(_addressObjects.transform, worldPositionStays: false); });
        }

    }

    private void LoadMob()
    {
        List<AssetReferenceGameObject> _stageMobs;
        switch (PlayerController.Instance.PlayerData.CurrentStage)
        {
            case 0:
                _stageMobs = _stage1Mobs;
                break;
            case 1:
                _stageMobs = _stage2Mobs;
                break;
            case 2:
                _stageMobs = _stage3Mobs;
                break;
            case 3:
                _stageMobs = _stage4Mobs;
                break;
            case 4:
                _stageMobs = _stage5Mobs;
                break;
            default:
                return;
        }

        for(int i= 0; i < _stageMobs.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.GetObjectAndSave(_stageMobs[index], ObjectPoolManager.Instance.Mobs, (obj) =>{ _clearLoadAssetCount++; obj.transform.SetParent(_addressObjects.transform, worldPositionStays: false); });
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Fill StageMob")]
    private void FillStageMob()
    {
        _stage1Mobs.Clear();

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
#endif
}
