using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : UIBInder
{
    #region Addressable Assets
    [Header("어드레서블 에셋 설정")]
    // 자주 쓰는 스프라이트 캐시
    private readonly Dictionary<string, Sprite> _spriteDic = new Dictionary<string, Sprite>();

    // 캐싱 키
    private const string SPR_BUTTON = "Button";
    private const string SPR_POPUP = "PopUpPanel";
    private const string SPR_BACK = "BackPanel";
    private const string SPR_DIAMOND = "Diamond";

    [SerializeField] private AssetReferenceSprite _buttonSprite;

    [SerializeField] private AssetReferenceSprite _popUpPanelSprite;
    [SerializeField] private AssetReferenceSprite _backPanelSprite;

    [SerializeField] private AssetReferenceSprite _diamondSprite;

    [SerializeField] private AssetReferenceSprite _pauseSprite;
    [SerializeField] private AssetReferenceSprite _soundOnSprite;
    [SerializeField] private AssetReferenceSprite _soundOffSprite;

    [SerializeField] private AssetReferenceGameObject _enhanceArcherSprite;
    [SerializeField] private AssetReferenceSprite _enhanceBomerSprite;
    [SerializeField] private AssetReferenceSprite _enhanceWarriorSprite;

    [SerializeField] private AssetReferenceSprite _sellNormalSprite;
    [SerializeField] private AssetReferenceSprite _sellRareSprite;
    [SerializeField] private AssetReferenceSprite _sellAncientSprite;
    [SerializeField] private AssetReferenceSprite _sellLegendSprite;
    [SerializeField] private AssetReferenceSprite _sellEpicSprite;

    [SerializeField] private AssetReferenceSprite _gameEndPanelNameSprite;
    [SerializeField] private AssetReferenceSprite _gameEndPanelButtonSprite;

    [SerializeField] private List<AssetReferenceSprite> _mapSprites;
    [SerializeField] private List<AssetReferenceSprite> _palletSprites;
    #endregion


    // =========================================================
    [Header("")]
    [Header("인게임 설정")]
    [SerializeField] WaveManager _waveManager;
    [SerializeField] RandomSpawnUnitController _randomUnitSpawner;
    [SerializeField] UnitSpawner _unitSpawnerController;
    [SerializeField] UnitEnhancer _unitEnhancer;

    // 특정 패널들 바인딩용
    [HideInInspector] public GameObject _warningPanel;
    [HideInInspector] public GameObject _warningPanel2;
    [HideInInspector] public GameObject _unitSellPanel;
    [HideInInspector] public GameObject _gameEndPanel;
    [HideInInspector] public GameObject _loadingPanel;

    private Button GameClearButton => GetUI<Button>("GameClearButton"); // Test용

    [Header("Top Panel")]
    private Button _stopButton;
    private Image _stopImage;
    private Button _speedButton;
    private Button _soundButton;
    private GameObject _soundOnImage;
    private GameObject _soundOffImage;
    private TMPro.TMP_Text _gameSpeedText;
    private Image _bossImage;
    private TMPro.TMP_Text _bossName;
    private TMPro.TMP_Text _waveText;
    private TMPro.TMP_Text _timeText;

    [Header("Bottom Panel")]
    private Slider _aliveMonsterCountSlider;
    private TMPro.TMP_Text _aliveMonsterCountText;
    private TMPro.TMP_Text _jewelText;
    private Button _randomSppawnButton;
    private Button _unitSellButton;
    private Button _enhanceArcherButton;
    private Button _enhanceBomerButton;
    private Button _enhanceWarriorButton;
    private Image _enhanceArcherImage;
    private Image _enhanceBomerImage;
    private Image _enhanceWarriorImage;

    [Header("Unit Sell Panel")]
    private TMPro.TMP_Text _normalAmountText;
    private TMPro.TMP_Text _rareAmountText;
    private TMPro.TMP_Text _ancientAmountText;
    private TMPro.TMP_Text _legendAmountText;
    private TMPro.TMP_Text _epicAmountText;
    private Button _closeSellUnitButton;
    private Button _sellNormalButton;
    private Button _sellRareButton;
    private Button _sellAncientButton;
    private Button _sellLegendButton;
    private Button _sellEpicButton;
    private Button _tribe1Button;
    private Button _tribe2Button;
    private Button _tribe3Button;
    private Image _normalImage;
    private Image _rareImage;
    private Image _ancientImage;
    private Image _legendImage;
    private Image _epicImage;

    [Header("Game End Panel")]
    private TMPro.TMP_Text _clearFailText;
    private TMPro.TMP_Text _recordWaveText;
    private TMPro.TMP_Text _recordClearTimeText;
    private Button _loadMainSceneButton;

    [Header("Map")]
    [SerializeField] Image _backgroundSprite;
    [SerializeField] Image _palletSprite;

    private Image _jewellImage1;
    private Image _jewellImage2;
    private Image _jewellImage3;
    private Image _jewellImage4;
    private Image _jewellImage5;
    private Image _jewellImage6;
    private Image _jewellImage7;
    private Image _jewellImage8;
    private Image _jewellImage9;
    private Image _jewellImage10;
    private Image _jewellImage11;


    // 현재 선택된 부족 인덱스(패널 갱신 시 사용)
    private int _currentSellIndex = 0;

    private void Awake()
    {
        BindAll();
        InitUI();
    }

    private void InitUI()
    {
        // 특정 패널들 바인딩용
        _warningPanel = GetUI("WarningPanel");
        _warningPanel2 = GetUI("WarningPanel2");
        _unitSellPanel = GetUI("UnitSellPanel");
        _gameEndPanel = GetUI("GameEndPanel");
        _loadingPanel = GetUI("LoadingPanel");


        // Top Panel
        _stopButton = GetUI<Button>("StopButton");
        _stopImage = GetUI<Image>("StopImage");
        AddressableManager.Instance.LoadSprite(_pauseSprite, _stopImage, () => { });
        _speedButton = GetUI<Button>("SpeedButton");
        _soundButton = GetUI<Button>("SoundButton");
        _soundOnImage = GetUI("SoundOnImage");
        AddressableManager.Instance.LoadSprite(_soundOnSprite, _soundOnImage.GetComponent<Image>(), () => { /* don't change active here */ SetSoundImage(); });
        _soundOffImage = GetUI("SoundOffImage");
        _soundOffImage = GetUI("SoundOffImage");

        AddressableManager.Instance.LoadSprite(_soundOffSprite, _soundOffImage.GetComponent<Image>(), () => { /* don't change active here */ SetSoundImage(); });
        _gameSpeedText = GetUI<TMPro.TMP_Text>("GameSpeedText");
        _bossImage = GetUI<Image>("MonsterImage"); // Image 컴포넌트 참조 저장
        _bossName = GetUI<TMPro.TMP_Text>("MonsterNameText");
        _waveText = GetUI<TMPro.TMP_Text>("WaveText");
        _timeText = GetUI<TMPro.TMP_Text>("TimeText");

        // Bottom Panel
        _aliveMonsterCountSlider = GetUI<Slider>("AliveMonsterCountSlider");
        _aliveMonsterCountText = GetUI<TMPro.TMP_Text>("CurMonsterCountText");

        _jewelText = GetUI<TMPro.TMP_Text>("JewelText");
        _randomSppawnButton = GetUI<Button>("RandomSpawnButton");
        _unitSellButton = GetUI<Button>("SellUnitButton");
        _enhanceArcherButton = GetUI<Button>("EnhanceArcherButton");
        _enhanceBomerButton = GetUI<Button>("EnhanceBomerButton");
        _enhanceWarriorButton = GetUI<Button>("EnhanceWarriorButton");

        _enhanceArcherImage = GetUI<Image>("EnhanceArcherImage");
        LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Archer/Ancient_archer_attack.prefab", _enhanceArcherImage, null);
        _enhanceBomerImage = GetUI<Image>("EnhanceBomerImage");
        LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Bomer/Ancient_bomer_attack.prefab", _enhanceBomerImage, null);
        _enhanceWarriorImage = GetUI<Image>("EnhanceWarriorImage");
        LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Warrior/Ancient_warrior_attack.prefab", _enhanceWarriorImage, null);


        // Unit Sell Panel
        _normalAmountText = GetUI<TMPro.TMP_Text>("NormalAmountText"); //TODO : 딕셔너리로 연결해서 수량 업데이트 Fix
        _rareAmountText = GetUI<TMPro.TMP_Text>("RareAmountText");
        _ancientAmountText = GetUI<TMPro.TMP_Text>("AncientAmountText");
        _legendAmountText = GetUI<TMPro.TMP_Text>("LegendAmountText");
        _epicAmountText = GetUI<TMPro.TMP_Text>("EpicAmountText");
        _closeSellUnitButton = GetUI<Button>("ClosePanelButton");
        _sellNormalButton = GetUI<Button>("SellNormalButton");
        _sellRareButton = GetUI<Button>("SellRareButton");
        _sellAncientButton = GetUI<Button>("SellAncientButton");
        _sellLegendButton = GetUI<Button>("SellLegendButton");
        _sellEpicButton = GetUI<Button>("SellEpicButton");
        _tribe1Button = GetUI<Button>("Tribe1Button"); // 버튼 클릭시 해당 부족 유닛만 보이게
        _tribe2Button = GetUI<Button>("Tribe2Button");
        _tribe3Button = GetUI<Button>("Tribe3Button");
        _normalImage = GetUI("NormalImage").GetComponent<Image>(); // 부족 버튼 이미지
        _rareImage = GetUI("RareImage").GetComponent<Image>();
        _ancientImage = GetUI("AncientImage").GetComponent<Image>();
        _legendImage = GetUI("LegendImage").GetComponent<Image>();
        _epicImage = GetUI("EpicImage").GetComponent<Image>();
        _closeSellUnitButton = GetUI<Button>("ClosePanelButton");

        // GaneEndPanel
        _clearFailText = GetUI<TMPro.TMP_Text>("ClearFailText");
        AddressableManager.Instance.LoadSprite(_gameEndPanelNameSprite, GetUI<Image>("ClearFailImage"), () => { });
        _recordWaveText = GetUI<TMPro.TMP_Text>("RecordWaveText");
        _recordClearTimeText = GetUI<TMPro.TMP_Text>("RecordClearTimeText");
        _loadMainSceneButton = GetUI<Button>("LoadMainSceneButton");
        AddressableManager.Instance.LoadSprite(_gameEndPanelButtonSprite, _loadMainSceneButton.GetComponent<Image>(), () => { });

        // 보석 이미지
        _jewellImage1 = GetUI<Image>("JewelImage1");
        _jewellImage2 = GetUI<Image>("JewelImage2");
        _jewellImage3 = GetUI<Image>("JewelImage3");
        _jewellImage4 = GetUI<Image>("JewelImage4");
        _jewellImage5 = GetUI<Image>("JewelImage5");
        _jewellImage6 = GetUI<Image>("JewelImage6");
        _jewellImage7 = GetUI<Image>("JewelImage7");
        _jewellImage8 = GetUI<Image>("JewelImage8");
        _jewellImage9 = GetUI<Image>("JewelImage9");
        _jewellImage10 = GetUI<Image>("JewelImage10");
        _jewellImage11 = GetUI<Image>("JewelImage11");

        ApplyButtonSprite();

        ApplyPopupSprite();

        ApplyBackPanelSprite();

        ApplyJewelImage();

        SetMapImage(PlayerController.Instance.PlayerData.CurrentStage);

        // 초기 음향 버튼 상태를 저장된 값에 맞춰 UI에 반영
        SetSoundImage();
    }

    private void Start()
    {
        GameClearButton.onClick.AddListener(() => TestClearButton()); // Test용

        // 프리로드 후 공통 스프라이트 적용
        /*StartCoroutine(PreloadCommonSpritesThenApply());*/

        // Top Panel
        _stopButton.onClick.AddListener(OnPauseGame);
        _speedButton.onClick.AddListener(OnSpeedButtonClicked);
        // 안전하게 null 체크 후 리스너 추가
        if (_soundButton != null)
            _soundButton.onClick.AddListener(OnSoundButtonClicked);
        WaveManager.Instance.OnChangeBoss += SetBossInfo;

        _waveManager.CurWaveChanged += OnWaveChanged;
        _waveManager.AliveMonsterCountChanged += OnAliveMonsterCountChanged;

        // Bottom Panel
        _waveManager.AliveMonsterCountChanged += SetAliveMonsterCountSlider;
        GameManager.Instance.JewelChanged += OnJewelChanged;
        OnJewelChanged(GameManager.Instance.Jewel);

        _randomSppawnButton.onClick.AddListener(_randomUnitSpawner.SpawnRandomUnit);
        _unitSellButton.onClick.AddListener(() =>
        {
            _unitSellPanel.SetActive(true);

            SetSellPanel(0);
        });

        _enhanceArcherButton.onClick.AddListener(() => _unitEnhancer.EnhanceArcher());
        _enhanceBomerButton.onClick.AddListener(() => _unitEnhancer.EnhanceBomer());
        _enhanceWarriorButton.onClick.AddListener(() => _unitEnhancer.EnhanceWarrior());

        // Unit Sell Panel
        _closeSellUnitButton.onClick.AddListener(() => _unitSellPanel.SetActive(false));
        _tribe1Button.onClick.AddListener(() => SetSellPanel(0));
        _tribe2Button.onClick.AddListener(() => SetSellPanel(1));
        _tribe3Button.onClick.AddListener(() => SetSellPanel(2));
        _sellNormalButton.onClick.AddListener(() =>
        {
            _unitSpawnerController.SellUnit(_randomUnitSpawner.NormalUnits[_currentSellIndex]);
            SetSellPanel(_currentSellIndex);
        });
        _sellRareButton.onClick.AddListener(() =>
        {
            _unitSpawnerController.SellUnit(_randomUnitSpawner.RareUnits[_currentSellIndex]);
            SetSellPanel(_currentSellIndex);
        });
        _sellAncientButton.onClick.AddListener(() =>
        {
            _unitSpawnerController.SellUnit(_randomUnitSpawner.AncientUnits[_currentSellIndex]);
            SetSellPanel(_currentSellIndex);
        });
        _sellLegendButton.onClick.AddListener(() =>
        {
            _unitSpawnerController.SellUnit(_randomUnitSpawner.LegendUnits[_currentSellIndex]);
            SetSellPanel(_currentSellIndex);
        });
        _sellEpicButton.onClick.AddListener(() =>
        {
            _unitSpawnerController.SellUnit(_randomUnitSpawner.EpicUnits[_currentSellIndex]);
            SetSellPanel(_currentSellIndex);
        });

        // 스폰/판매 시 수량 갱신
        if (_unitSpawnerController != null)
            _unitSpawnerController.UnitsCountChanged += RefreshSellPanelIfOpen;

        // GameEndPanel
        GameManager.Instance.SetGameEndHandler += SetGameEndPanel;
        _loadMainSceneButton.onClick.AddListener(() => SceneManager.LoadScene(1));

        SetBossInfo();
        SetAliveMonsterCountSlider(0);
        SetGameSpeedText();

        // 안전하게 한 번 더 동기화 (Addressable 로드가 완전히 끝나기 전에 UI 상태를 보장)
        SetSoundImage();
    }

    private void OnDestroy()
    {
        // Top Panel
        _stopButton.onClick.RemoveAllListeners();
        _speedButton.onClick.RemoveListener(OnSpeedButtonClicked);
        if (_soundButton != null)
            _soundButton.onClick.RemoveListener(OnSoundButtonClicked);

        _waveManager.CurWaveChanged -= OnWaveChanged;
        _waveManager.AliveMonsterCountChanged -= OnAliveMonsterCountChanged;

        // Bottom Panel
        if (GameManager.Instance != null)
            GameManager.Instance.JewelChanged -= OnJewelChanged;

        _randomSppawnButton.onClick.RemoveListener(_randomUnitSpawner.SpawnRandomUnit);
        _unitSellButton.onClick.RemoveAllListeners();

        // Unit Sell Panel
        _closeSellUnitButton.onClick.RemoveAllListeners();
        _tribe1Button.onClick.RemoveAllListeners();
        _tribe2Button.onClick.RemoveAllListeners();
        _tribe3Button.onClick.RemoveAllListeners();

        // GameEndPanel
        GameManager.Instance.SetGameEndHandler -= SetGameEndPanel;
        _loadMainSceneButton.onClick.RemoveAllListeners();

        if (_unitSpawnerController != null)
            _unitSpawnerController.UnitsCountChanged -= RefreshSellPanelIfOpen;
    }

    private void Update()
    {
        _timeText.text = $"{System.TimeSpan.FromSeconds(GameManager.Instance.ClearTime):hh\\:mm\\:ss}";
    }

    public void TestClearButton()
    {
        _waveManager.DeadMonsterCount = _waveManager.ClearCondition - 1;
    }

    private void DisableAllButtons(bool excludeStop = true)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (excludeStop && btn == _stopButton) continue;
            btn.interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
    }

    private void OnSpeedButtonClicked()
    {
        GameManager.Instance.SetGameSpeed();
        SetGameSpeedText();
    }

    public void OnSoundButtonClicked()
    {
        GameManager.Instance.SetSound();
        SetSoundImage();
        Debug.Log($"[InGameUI] OnSoundButtonClicked: IsSound = {PlayerController.Instance.PlayerData.IsSound}");
    }

    private void SetSoundImage()
    {
        // Player 데이터 기반으로 UI 동기화 (Addressable 로드 완료 여부와 무관하게 활성화 상태만 맞춤)
        bool isSound = PlayerController.Instance.PlayerData.IsSound;

        if (_soundOnImage != null) _soundOnImage.SetActive(isSound);
        if (_soundOffImage != null) _soundOffImage.SetActive(!isSound);
    }

    private void SetGameSpeedText()
    {
        _gameSpeedText.text = $"{GameManager.Instance.CurrentGameSpeed}X";
    }

    // 웨이브 변경 시 호출될 콜백
    // TODO : 맵 이미지 변경 추가
    public void OnWaveChanged(int wave)
    {
        _waveText.text = $"WAVE {wave}";
    }

    public void OnAliveMonsterCountChanged(int count)
    {
        _aliveMonsterCountText.text = $"{count}/50";
    }

    private void AddPanelList(params GameObject[] panel)
    {
        foreach (var p in panel)
        {
            UIManager.Instance.Panels.Add(p);
        }
    }

    private void OnJewelChanged(int jewel)
    {
        _jewelText.text = jewel.ToString();
    }

    public void OnPauseGame()
    {
        GameManager.Instance.PauseGame();
        if (GameManager.Instance.IsPause)
            DisableAllButtons(excludeStop: true);
        else
            EnableAllButtons();
    }

    // 유닛 판매 패널의 타입 설정
    public void SetSellPanel(int index)
    {
        _currentSellIndex = index;

        // 보유 수량 표시
        _normalAmountText.text = GetCountUnit(_randomUnitSpawner.NormalUnits, index).ToString();
        _rareAmountText.text = GetCountUnit(_randomUnitSpawner.RareUnits, index).ToString();
        _ancientAmountText.text = GetCountUnit(_randomUnitSpawner.AncientUnits, index).ToString();
        _legendAmountText.text = GetCountUnit(_randomUnitSpawner.LegendUnits, index).ToString();
        _epicAmountText.text = GetCountUnit(_randomUnitSpawner.EpicUnits, index).ToString();

        //TODO : 부족 버튼 이미지 변경
        switch (index)
        {
            case 0:
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Archer/Normal_archer_idle.prefab", _normalImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Archer/Rare_archer_idle.prefab", _rareImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Archer/Ancient_archer_idle.prefab", _ancientImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Archer/Legend_archer_idle.prefab", _legendImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Epic/Epic_archer_idle.prefab", _epicImage);
                break;
            case 1:
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Bomer/Normal_bomer_idle.prefab", _normalImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Bomer/Rare_bomer_idle.prefab", _rareImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Bomer/Ancient_bomer_idle.prefab", _ancientImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Bomer/Legend_bomer_idle.prefab", _legendImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Epic/Epic_bomer_idle.prefab", _epicImage);
                break;
            case 2:
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Warrior/Normal_warrior_idle.prefab", _normalImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Warrior/Rare_warrior_idle.prefab", _rareImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Warrior/Ancient_warrior_idle.prefab", _ancientImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Warrior/Legend_warrior_idle.prefab", _legendImage);
                LoadSpriteFromAddressablePrefab("Assets/Prefabs/OJH/Units/Epic/Epic_warrior_idle.prefab", _epicImage);
                break;
            default:
                Debug.LogWarning($"[InGameUI] SetSellPanel: 잘못된 인덱스 {index}");
                break;
        }
    }

    private int GetCountUnit(GameObject[] group, int idx)
    {
        if (group == null)
        {
            Debug.LogWarning("[InGameUI] GetCountUnit: group 배열이 null");
            return 0;
        }
        if (idx < 0 || idx >= group.Length)
        {
            Debug.LogWarning($"[InGameUI] GetCountUnit: 인덱스 범위 초과 idx={idx}, len={group.Length}");
            return 0;
        }

        var prefab = group[idx];
        if (prefab == null) return 0;

        // 딕셔너리 존재/키 확인
        var dic = _unitSpawnerController.UnitsCountDic;
        if (dic == null) return 0;

        return dic.TryGetValue(prefab, out var count) ? count : 0;
    }

    private void RefreshSellPanelIfOpen()
    {
        if (_unitSellPanel != null && _unitSellPanel.activeSelf)
            SetSellPanel(_currentSellIndex);
    }

    public void SetAliveMonsterCountSlider(int count)
    {
        _aliveMonsterCountSlider.maxValue = 50;
        _aliveMonsterCountSlider.value = count;
    }

    private void SetGameEndPanel()
    {
        _clearFailText.text = (PlayerController.Instance.PlayerData.IsClearStage[PlayerController.Instance.PlayerData.CurrentStage]) ? "클리어 성공" : "스테이지 실패";
        _recordWaveText.text = $"{_waveManager.CurWave} 웨이브";
        _recordClearTimeText.text = $"{System.TimeSpan.FromSeconds(GameManager.Instance.ClearTime):hh\\.mm\\.ss}";

        Debug.Log($"[InGameUI] 게임 엔드 페널 -  클리어/실패 돌려 쓰는 중, 결과 : {PlayerController.Instance.PlayerData.IsClearStage[PlayerController.Instance.PlayerData.CurrentStage]}");
    }

    //0.5425906
    //0.6

    private void SetBossInfo()
    {
        int curStage = PlayerController.Instance.PlayerData.CurrentStage;
        string key = $"Assets/Prefabs/OJH/Monsters/Boss/Stage{curStage + 1}_Boss.prefab";

        LoadSpriteFromAddressablePrefab(key, _bossImage, null);
            _bossName.text = WaveManager.Instance.BossMonsterName[curStage];
    }

    private void SetMapImage(int stage)
    {
        Image mapImageComp = _backgroundSprite.GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_mapSprites[stage], mapImageComp, () => { });

        Image palletImageComp = _palletSprite.GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_palletSprites[stage], palletImageComp, () => { _palletSprite.gameObject.SetActive(false); });
    }

    private void ApplyButtonSprite()
    {
        // 캐시 먼저 확인
        if (_spriteDic.TryGetValue(SPR_BUTTON, out var cachedBtnSprite) && cachedBtnSprite != null)
        {
            Image stopImg = _stopButton.GetComponent<Image>();
            stopImg.sprite = cachedBtnSprite;
        }
        else
        {
            Image stopImg = _stopButton.GetComponent<Image>();
            AddressableManager.Instance.LoadSprite(
                _buttonSprite,
                stopImg,
                () =>
                {
                    var loaded = stopImg.sprite; // 매개변수 없이 target Image에서 읽음
                    if (loaded == null) return;
                    _spriteDic[SPR_BUTTON] = loaded;

                    // Top Panel
                    _speedButton.GetComponent<Image>().sprite = loaded;
                    _soundButton.GetComponent<Image>().sprite = loaded;

                    // Bottom Panel
                    GetUI("CurJewelPanel").GetComponent<Image>().sprite = loaded;
                    _randomSppawnButton.GetComponent<Image>().sprite = loaded;
                    _unitSellButton.GetComponent<Image>().sprite = loaded;
                    _enhanceArcherButton.GetComponent<Image>().sprite = loaded;
                    _enhanceBomerButton.GetComponent<Image>().sprite = loaded;
                    _enhanceWarriorButton.GetComponent<Image>().sprite = loaded;
                    GetUI<Image>("SpecialSpawnButton").sprite = loaded;

                    // Unit Sell Panel
                    _sellNormalButton.GetComponent<Image>().sprite = loaded;
                    _sellRareButton.GetComponent<Image>().sprite = loaded;
                    _sellAncientButton.GetComponent<Image>().sprite = loaded;
                    _sellLegendButton.GetComponent<Image>().sprite = loaded;
                    _sellEpicButton.GetComponent<Image>().sprite = loaded;
                    _closeSellUnitButton.GetComponent<Image>().sprite = loaded;

                    // Wave Panel
                    GetUI("WavePanel").GetComponent<Image>().sprite = loaded;
                }
            );
        }
    }

    public void ApplyPopupSprite()
    {
        // 캐시 먼저 확인
        if (_spriteDic.TryGetValue(SPR_BACK, out var cachedBackSprite) && cachedBackSprite != null)
        {

        }
        else
        {
            Image sellPanel = GetUI("UnitSellPanel").GetComponent<Image>();
            AddressableManager.Instance.LoadSprite(
                _popUpPanelSprite,
                sellPanel,
                () =>
                {
                    var loaded = sellPanel.sprite; // 매개변수 없이 target Image에서 읽음
                    if (loaded == null) return;
                    _spriteDic[SPR_BACK] = loaded;
                    // 게임 종료 패널
                    Image gameEndPanel = GetUI("GameEndPanel").GetComponent<Image>();
                    gameEndPanel.sprite = loaded;
                    sellPanel.gameObject.SetActive(false);
                }
            );
        }
    }

    public void ApplyBackPanelSprite()
    {
        // 캐시 먼저 확인
        if (_spriteDic.TryGetValue(SPR_POPUP, out var cachedPopUpSprite) && cachedPopUpSprite != null)
        {

        }
        else
        {
            Image tribeButton = _tribe1Button.GetComponent<Image>();
            AddressableManager.Instance.LoadSprite(
                _backPanelSprite,
                tribeButton,
                () =>
                {
                    var loaded = tribeButton.sprite; // 매개변수 없이 target Image에서 읽음
                    if (loaded == null) return;
                    _spriteDic[SPR_POPUP] = loaded;
                    // 부족 버튼
                    _tribe2Button.GetComponent<Image>().sprite = loaded;
                    _tribe3Button.GetComponent<Image>().sprite = loaded;
                    _warningPanel.GetComponent<Image>().sprite = loaded;
                    _warningPanel2.GetComponent<Image>().sprite = loaded;
                }
            );
        }
    }

    private void LoadSpriteFromAddressablePrefab(string prefabKey, Image target, System.Action onComplete = null)
    {
        if (target == null || string.IsNullOrEmpty(prefabKey))
        {
            Debug.LogWarning($"[InGameUI] LoadSpriteFromAddressablePrefab: 잘못된 인자 target={target}, key={prefabKey}");
            return;
        }

        UnityEngine.AddressableAssets.Addressables
            .LoadAssetAsync<GameObject>(prefabKey)
            .Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var prefab = handle.Result;
                    if (prefab == null)
                    {
                        Debug.LogWarning($"[InGameUI] LoadSpriteFromAddressablePrefab: 로드된 프리팹이 null - key={prefabKey}");
                        return;
                    }

                    Sprite sprite = null;

                    // 1) SpriteRenderer에서 스프라이트 추출 (자기 자신 -> 자식 포함)
                    var sr = prefab.GetComponent<SpriteRenderer>() ?? prefab.GetComponentInChildren<SpriteRenderer>(true);
                    if (sr != null && sr.sprite != null)
                    {
                        sprite = sr.sprite;
                    }

                    if (sprite != null)
                    {
                        target.sprite = sprite;
                    }
                    else
                    {
                        Debug.LogWarning($"[InGameUI] LoadSpriteFromAddressablePrefab: SpriteRenderer/Image에 Sprite가 없음 - key={prefabKey}");
                    }
                }
                else
                {
                    Debug.LogError($"[InGameUI] LoadSpriteFromAddressablePrefab: Addressable 로드 실패 - key={prefabKey}");
                }
            };
    }

    private void ApplyJewelImage()
    {
        // 캐시 사용
        if (_spriteDic.TryGetValue(SPR_DIAMOND, out var cached) && cached != null)
        {
            ApplyToJewelImages(cached);
            return;
        }

        var diaImage = _jewellImage1;
        if (diaImage == null)
        {
            Debug.LogWarning("[InGameUI] ApplyJewelImage: _jewellImage1(Image) 바인딩 실패");
            return;
        }
        if (_diamondSprite == null || !_diamondSprite.RuntimeKeyIsValid())
        {
            Debug.LogWarning("[InGameUI] ApplyJewelImage: _diamondSprite 참조가 유효하지 않음");
            return;
        }

        AddressableManager.Instance.LoadSprite(
            _diamondSprite,
            diaImage,
            () =>
            {
                // InGameUI가 파괴되었거나 대상 Image가 사라진 경우 방어
                if (this == null || diaImage == null) return;

                var loaded = diaImage.sprite;
                if (loaded == null) return;

                _spriteDic[SPR_DIAMOND] = loaded;
                ApplyToJewelImages(loaded);
            }
        );
    }

    private void ApplyToJewelImages(Sprite s)
    {
        if (s == null) return;
        SetImageSpriteSafe(_jewellImage1, s);
        SetImageSpriteSafe(_jewellImage2, s);
        SetImageSpriteSafe(_jewellImage3, s);
        SetImageSpriteSafe(_jewellImage4, s);
        SetImageSpriteSafe(_jewellImage5, s);
        SetImageSpriteSafe(_jewellImage6, s);
        SetImageSpriteSafe(_jewellImage7, s);
        SetImageSpriteSafe(_jewellImage8, s);
        SetImageSpriteSafe(_jewellImage9, s);
        SetImageSpriteSafe(_jewellImage10, s);
        SetImageSpriteSafe(_jewellImage11, s);
    }

    private static void SetImageSpriteSafe(Image img, Sprite s)
    {
        if (img != null)
        {
            img.sprite = s;
            if (img.gameObject != null) img.gameObject.SetActive(true);
        }
    }


}
