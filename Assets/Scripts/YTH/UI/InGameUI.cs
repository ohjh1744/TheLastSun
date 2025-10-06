using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : UIBInder
{
    [SerializeField] WaveManager _monsterSpawnmer;
    [SerializeField] WaveManager _waveManager;
    [SerializeField] RandomSpawnUnitController _randomUnitSpawner;
    [SerializeField] UnitSpawner _unitSpawnerController;


    // 특정 패널들 바인딩용
    [HideInInspector] public GameObject _warningPanel;
    [HideInInspector] public GameObject _warningPanel2;
    [HideInInspector] public GameObject _unitSellPanel;
    [HideInInspector] public GameObject _gameEndPanel;

    [Header("Top Panel")]
    private Button _stopButton;
    private Button _speedButton;
    private Button _soundButton;
    private GameObject _soundOnImage;
    private GameObject _soundOffImage;
    private TMPro.TMP_Text _gameSpeedText;
    private Image _bossImage; // Sprite -> Image로 변경
    private TMPro.TMP_Text _bossName;
    private TMPro.TMP_Text _waveText;
    private TMPro.TMP_Text _timeText;

    [Header("Bottom Panel")]
    private Slider _aliveMonsterCountSlider;
    private TMPro.TMP_Text _aliveMonsterCountText;
    private TMPro.TMP_Text _jewelText;
    private Button _randomSppawnButton;
    private Button _unitSellButton;

    // Unit Sell Panel
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
    private Sprite _normalImage;
    private Sprite _rareImage;
    private Sprite _ancientImage;
    private Sprite _legendImage;
    private Sprite _epicImage;

    // GameEndPanel
    private TMPro.TMP_Text _clearFailText;
    private TMPro.TMP_Text _recordWaveText;
    private TMPro.TMP_Text _recordClearTimeText;
    private Button _loadMainSceneButton;

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

        // Top Panel
        _stopButton = GetUI<Button>("StopButton");
        _speedButton = GetUI<Button>("SpeedButton");
        _soundButton = GetUI<Button>("SoundButton");
        _soundOnImage = GetUI("SoundOnImage");
        _soundOffImage = GetUI("SoundOffImage");
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
        _normalImage = GetUI("NormalImage").GetComponent<Image>().sprite; // 부족 버튼 이미지
        _rareImage = GetUI("RareImage").GetComponent<Image>().sprite;
        _ancientImage = GetUI("AncientImage").GetComponent<Image>().sprite;
        _legendImage = GetUI("LegendImage").GetComponent<Image>().sprite;
        _epicImage = GetUI("EpicImage").GetComponent<Image>().sprite;

        // GaneEndPanel
        _clearFailText = GetUI<TMPro.TMP_Text>("ClearFailText");
        _recordWaveText = GetUI<TMPro.TMP_Text>("RecordWaveText");
        _recordClearTimeText = GetUI<TMPro.TMP_Text>("RecordClearTimeText");
        _loadMainSceneButton = GetUI<Button>("LoadMainSceneButton");
    }

    private void Start()
    {
        // Top Panel
        _stopButton.onClick.AddListener(OnPauseGame);

        _speedButton.onClick.AddListener(OnSpeedButtonClicked);
        _soundButton.onClick.AddListener(OnSoundButtonClicked);

        WaveManager.Instance.OnChangeBoss += SetBossInfo;
        
        _monsterSpawnmer.CurWaveChanged += OnWaveChanged;
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
    }

    private void OnDestroy()
    {
        // Top Panel
        _stopButton.onClick.RemoveAllListeners();
        _speedButton.onClick.RemoveListener(OnSpeedButtonClicked);
        _soundButton.onClick.RemoveAllListeners();

        _monsterSpawnmer.CurWaveChanged -= OnWaveChanged;
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

    private void OnSoundButtonClicked()
    {
        GameManager.Instance.SetSound();
        if (PlayerController.Instance.PlayerData.IsSound)
        {
            _soundOnImage.SetActive(true);
            _soundOffImage.SetActive(false);
        }
        else
        {
            _soundOnImage.SetActive(false);
            _soundOffImage.SetActive(true);
        }
    }

    private void SetGameSpeedText()
    {
        _gameSpeedText.text = $"{GameManager.Instance.CurrentGameSpeed}X";
    }

    // 웨이브 변경 시 호출될 콜백
    public void OnWaveChanged(int wave)
    {
        _waveText.text = $"Wave {wave}";
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
        var dic = _unitSpawnerController.UnitsCountDic; // 프로젝트에 존재하는 딕셔너리 이름 사용
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
    }

    private void SetBossInfo()
    {
        _bossImage.sprite = WaveManager.Instance.BossPrefabs[WaveManager.Instance.ToSpawnBossindex].GetComponent<SpriteRenderer>().sprite;
        _bossName.text = WaveManager.Instance.BossPrefabs[WaveManager.Instance.ToSpawnBossindex].name;
    }
}
