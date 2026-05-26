using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class OutAddrContainer : UIBInder, IAssetLoadable
{
    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }

    private readonly List<AssetReference> _loadedAssets = new List<AssetReference>();
    private const int LoadRetryCount = 3;
    #endregion

    #region Addressable Assets
    //UI
    [Header("UI")]
    [SerializeField] AssetReferenceSprite _button_02;
    [SerializeField] AssetReferenceSprite _button_03;
    [SerializeField] AssetReferenceSprite _button_04;
    [SerializeField] AssetReferenceSprite _cancel;
    [SerializeField] AssetReferenceSprite _frame;
    [SerializeField] AssetReferenceSprite _icon_Lock;
    [SerializeField] AssetReferenceSprite _popUp;
    [SerializeField] AssetReferenceSprite _popUp_Title;
    [SerializeField] AssetReferenceSprite _difficultyImage;
    [SerializeField] AssetReferenceSprite _stageChangeLeftButton;
    [SerializeField] AssetReferenceSprite _stageChangeRightButton;
    [SerializeField] AssetReferenceSprite _resourceBar1;
    [SerializeField] List<AssetReferenceSprite> _backgroundSprites;
    [SerializeField] List<AssetReferenceSprite> _stageSprites;
    [SerializeField] List<AssetReferenceSprite> _bossLocSprites;
    [SerializeField] List<AssetReferenceSprite> _bossUnLocSprites;
    [SerializeField] AssetReferenceT<AudioClip> _bgm;

    #endregion

    void Awake()
    {
        BindAll();
    }
    void Start()
    {
        StartCoroutine(LoadAssetSequential());
        //LoadAsset();
    }

    void LoadAsset()
    {
        AddressableManager.Instance.LoadOnlySprite(_button_02, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("PlayButton").sprite = sprite;
            GetUI<Image>("CheckRankButton").sprite = sprite;
            GetUI<Image>("BossBookButton").sprite = sprite;
            GetUI<Image>("SettingButton").sprite = sprite;

        });
        AddressableManager.Instance.LoadOnlySprite(_button_03, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("SetMusicButton").sprite = sprite;
            GetUI<Image>("ShowCreditButton").sprite = sprite;
            GetUI<Image>("ReviewButton").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_button_04, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossBookSetFalseBgImage").sprite = sprite;
            GetUI<Image>("SettingSetFalseBgImage").sprite = sprite;
            GetUI<Image>("BossBookScrollView").sprite = sprite;
            GetUI<Image>("BossBookExplainBgImage").sprite = sprite;
            GetUI<Image>("CreditSetFalseBgImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_cancel, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossBookSetFalseButton").sprite = sprite;
            GetUI<Image>("SettingSetFalseButton").sprite = sprite;
            GetUI<Image>("CreditSetFalseButton").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_frame, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossPortraitBgImage1").sprite = sprite;
            GetUI<Image>("BossPortraitBgImage2").sprite = sprite;
            GetUI<Image>("BossPortraitBgImage3").sprite = sprite;
            GetUI<Image>("BossPortraitBgImage4").sprite = sprite;
            GetUI<Image>("BossPortraitBgImage5").sprite = sprite;
            GetUI<Image>("BossBookExplainPortraitBgImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_icon_Lock, (sprite) => { 
            _clearLoadAssetCount++; 
            GetUI<Image>("LockImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_popUp, (sprite) =>{
            _clearLoadAssetCount++;
            GetUI<Image>("SettingPanel").sprite = sprite;
            GetUI<Image>("CreditPanel").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_popUp_Title, (sprite) => { 
            _clearLoadAssetCount++; 
            GetUI<Image>("SettingNameBgImage").sprite = sprite;
            GetUI<Image>("BossBookStateImage").sprite = sprite;
        });
        AddressableManager.Instance.LoadOnlySprite(_difficultyImage, (sprite) => {
            _clearLoadAssetCount++;
            for (int i = 0; i < 6; i++)
            {
                GetUI<Image>($"DifficultyLevelImage{i + 1}").sprite = sprite;
            }
            if(PlayerController.Instance.PlayerData.CurrentStage == 0)
            {
                GetUI("DifficultyLevel1Images").SetActive(true);
            }
            else if(PlayerController.Instance.PlayerData.CurrentStage == 1 || PlayerController.Instance.PlayerData.CurrentStage == 2)
            {
                GetUI("DifficultyLevel2Images").SetActive(true);
            }
            else if (PlayerController.Instance.PlayerData.CurrentStage == 3 || PlayerController.Instance.PlayerData.CurrentStage == 4)
            {
                GetUI("DifficultyLevel3Images").SetActive(true);
            }
        });
        AddressableManager.Instance.LoadOnlySprite(_stageChangeLeftButton, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("StageChangeLeftButton").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_stageChangeRightButton, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("StageChangeRightButton").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_resourceBar1, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("BossBookExplainTextBgImage").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_backgroundSprites[0], (sprite) => { _clearLoadAssetCount++; GetUI<Image>("MainPanel").sprite = sprite; });
        AddressableManager.Instance.LoadOnlySprite(_backgroundSprites[1], (sprite) => { _clearLoadAssetCount++; GetUI<Image>("BossBookPanel").sprite = sprite; });
        for(int i = 0; i < _stageSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_stageSprites[index], (sprite) => { 
                _clearLoadAssetCount++; 
                GetUI<Image>($"Stage{index + 1}Image").sprite = sprite;
                if(PlayerController.Instance.PlayerData.CurrentStage == index)
                {
                    
                    GetUI($"Stage{index + 1}Image").SetActive(true);
                }
            });
        }
        for (int i = 0; i< _bossLocSprites.Count; i++)
        {
            int index = i;
            if (PlayerController.Instance.PlayerData.IsClearStage[index] == true)
            {
                AddressableManager.Instance.LoadOnlySprite(_bossUnLocSprites[index], (sprite) => { _clearLoadAssetCount++; GetUI<Image>($"BossPortraitButton{index + 1}").sprite = sprite; });
            }
            else
            {
                AddressableManager.Instance.LoadOnlySprite(_bossLocSprites[index], (sprite) => { _clearLoadAssetCount++; GetUI<Image>($"BossPortraitButton{index + 1}").sprite = sprite; });
            }                   
        }
        AddressableManager.Instance.LoadSound(_bgm, GetUI<AudioSource>("Bgm"), () =>
        {
            _clearLoadAssetCount++;
            if (PlayerController.Instance.PlayerData.IsSound == true) {
                GetUI<AudioSource>("Bgm").Play();
            }
        });
    }

    IEnumerator LoadAssetSequential()
    {
        yield return AddressableManager.Instance.WaitUntilInitialized();

        if (!AddressableManager.Instance.IsInitialized)
            yield break;

        yield return LoadSpriteTask(_button_02, sprite =>
        {
            GetUI<Image>("PlayButton").sprite = sprite;
            GetUI<Image>("CheckRankButton").sprite = sprite;
            GetUI<Image>("BossBookButton").sprite = sprite;
            GetUI<Image>("SettingButton").sprite = sprite;
            _clearLoadAssetCount++;
        });

        yield return LoadSpriteTask(_button_03, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("SetMusicButton").sprite = sprite;
            GetUI<Image>("ShowCreditButton").sprite = sprite;
            GetUI<Image>("ReviewButton").sprite = sprite;
        });

        yield return LoadSpriteTask(_button_04, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossBookSetFalseBgImage").sprite = sprite;
            GetUI<Image>("SettingSetFalseBgImage").sprite = sprite;
            GetUI<Image>("BossBookScrollView").sprite = sprite;
            GetUI<Image>("BossBookExplainBgImage").sprite = sprite;
            GetUI<Image>("CreditSetFalseBgImage").sprite = sprite;
        });

        yield return LoadSpriteTask(_cancel, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossBookSetFalseButton").sprite = sprite;
            GetUI<Image>("SettingSetFalseButton").sprite = sprite;
            GetUI<Image>("CreditSetFalseButton").sprite = sprite;
        });

        yield return LoadSpriteTask(_frame, sprite =>
        {
            _clearLoadAssetCount++;
            for (int i = 1; i <= 5; i++)
                GetUI<Image>($"BossPortraitBgImage{i}").sprite = sprite;

            GetUI<Image>("BossBookExplainPortraitBgImage").sprite = sprite;
        });

        yield return LoadSpriteTask(_icon_Lock, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("LockImage").sprite = sprite;
        });

        yield return LoadSpriteTask(_popUp, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("SettingPanel").sprite = sprite;
            GetUI<Image>("CreditPanel").sprite = sprite;
        });

        yield return LoadSpriteTask(_popUp_Title, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("SettingNameBgImage").sprite = sprite;
            GetUI<Image>("BossBookStateImage").sprite = sprite;
        });

        yield return LoadSpriteTask(_difficultyImage, sprite =>
        {
            _clearLoadAssetCount++;

            for (int i = 0; i < 6; i++)
                GetUI<Image>($"DifficultyLevelImage{i + 1}").sprite = sprite;

            int stage = PlayerController.Instance.PlayerData.CurrentStage;

            if (stage == 0)
                GetUI("DifficultyLevel1Images").SetActive(true);
            else if (stage <= 2)
                GetUI("DifficultyLevel2Images").SetActive(true);
            else
                GetUI("DifficultyLevel3Images").SetActive(true);
        });

        yield return LoadSpriteTask(_stageChangeLeftButton, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("StageChangeLeftButton").sprite = sprite;
        });

        yield return LoadSpriteTask(_stageChangeRightButton, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("StageChangeRightButton").sprite = sprite;
        });

        yield return LoadSpriteTask(_resourceBar1, sprite =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("BossBookExplainTextBgImage").sprite = sprite;
        });

        // background
        for (int i = 0; i < _backgroundSprites.Count; i++)
        {
            int index = i;
            yield return LoadSpriteTask(_backgroundSprites[index], sprite =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>(index == 0 ? "MainPanel" : "BossBookPanel").sprite = sprite;
            });
        }

        // stage sprites
        for (int i = 0; i < _stageSprites.Count; i++)
        {
            int index = i;
            yield return LoadSpriteTask(_stageSprites[index], sprite =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>($"Stage{index + 1}Image").sprite = sprite;

                if (PlayerController.Instance.PlayerData.CurrentStage == index)
                    GetUI($"Stage{index + 1}Image").SetActive(true);
            });
        }

        // boss sprites
        for (int i = 0; i < _bossLocSprites.Count; i++)
        {
            int index = i;
            var target = PlayerController.Instance.PlayerData.IsClearStage[index]
                ? _bossUnLocSprites[index]
                : _bossLocSprites[index];

            yield return LoadSpriteTask(target, sprite =>
            {
                _clearLoadAssetCount++;
                GetUI<Image>($"BossPortraitButton{index + 1}").sprite = sprite;
            });
        }

        // sound
        yield return LoadSoundTask(_bgm, GetUI<AudioSource>("Bgm"), () =>
        {
            _clearLoadAssetCount++;

            if (PlayerController.Instance.PlayerData.IsSound)
                GetUI<AudioSource>("Bgm").Play();
        });

    }
    IEnumerator LoadSpriteTask(AssetReferenceSprite asset, Action<Sprite> onLoaded)
    {
        for (int retry = 0; retry < LoadRetryCount; retry++)
        {
            bool done = false;
            Sprite loadedSprite = null;

            AddressableManager.Instance.LoadOnlySprite(asset, sprite =>
            {
                loadedSprite = sprite;
                done = true;
            });

            yield return new WaitUntil(() => done);

            if (loadedSprite != null)
            {
                RegisterLoadedAsset(asset);

                try
                {
                    onLoaded?.Invoke(loadedSprite);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[OutAddrContainer] Sprite apply failed. Key: {asset.RuntimeKey}, Error: {exception}");
                }

                yield break;
            }

            asset.ReleaseAsset();
            Debug.LogWarning($"[OutAddrContainer] Sprite load retry {retry + 1}/{LoadRetryCount}. Key: {asset.RuntimeKey}");
            yield return null;
        }

        Debug.LogError($"[OutAddrContainer] Sprite final load failed. Key: {asset.RuntimeKey}");
    }

    IEnumerator LoadSoundTask(AssetReferenceT<AudioClip> clip, AudioSource source, Action onDone)
    {
        for (int retry = 0; retry < LoadRetryCount; retry++)
        {
            bool done = false;
            bool success = false;

            AddressableManager.Instance.LoadSound(clip, source, () =>
            {
                success = source != null && source.clip != null;
                done = true;
            });

            yield return new WaitUntil(() => done);

            if (success)
            {
                RegisterLoadedAsset(clip);
                onDone?.Invoke();
                yield break;
            }

            clip.ReleaseAsset();
            Debug.LogWarning($"[OutAddrContainer] Sound load retry {retry + 1}/{LoadRetryCount}. Key: {clip.RuntimeKey}");
            yield return null;
        }

        Debug.LogError($"[OutAddrContainer] Sound final load failed. Key: {clip.RuntimeKey}");
    }

    private void RegisterLoadedAsset(AssetReference asset)
    {
        if (asset != null && !_loadedAssets.Contains(asset))
            _loadedAssets.Add(asset);
    }

    private void OnDestroy()
    {
        for (int i = _loadedAssets.Count - 1; i >= 0; i--)
        {
            if (_loadedAssets[i] != null)
                _loadedAssets[i].ReleaseAsset();
        }

        _loadedAssets.Clear();
    }
}
