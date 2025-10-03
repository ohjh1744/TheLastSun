using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIBInder
{
    [SerializeField] WaveManager _monsterSpawnmer;
    [SerializeField] WaveManager _waveManager;
    [SerializeField] RandomSpawnUnitController _randomUnitSpawner;
    [SerializeField] UnitSpawner _unitSpawnerController;

    [SerializeField] GameObject _targetUnit;

    // 특정 패널들 바인딩용
    [HideInInspector] public GameObject _warningPanel;
    [HideInInspector] public GameObject _gameOverPanel;
    [HideInInspector] public GameObject _unitSellPanel;
    [HideInInspector] public GameObject _clearPanel;

    [Header("Top Panel")]
    private Button _stopButton;
    private Button _speedButton;
    private TMPro.TMP_Text _gameSpeedText;
    private TMPro.TMP_Text _monsterNameText;
    private TMPro.TMP_Text _waveText;
    private TMPro.TMP_Text _timeText;

    [Header("Bottom Panel")]
    private TMPro.TMP_Text _curMonsterCountText;
    private TMPro.TMP_Text _jewelText;
    private Button _randomSppawnButton;
    private Button _unitSellButton;

    // Unit Sell Panel
    private TMPro.TMP_Text _normalText;
    private TMPro.TMP_Text _rareText;
    private TMPro.TMP_Text _ancientText;
    private TMPro.TMP_Text _legendText;
    private TMPro.TMP_Text _epicText;
    private Button _closeSellUnitButton;
    private Button _sellNornalButton;
    private Button _tribe1Button;
    private Button _tribe2Button;
    private Button _tribe3Button;

    private void Awake()
    {
        BindAll();
        InitUI();
    }

    private void InitUI()
    {
        // 특정 패널들 바인딩용
        _warningPanel = GetUI("WarningPanel");
        _gameOverPanel = GetUI("GameOverPanel");
        _unitSellPanel = GetUI("UnitSellPanel");
        _clearPanel = GetUI("ClearPanel");

        // Top Panel
        _stopButton = GetUI<Button>("StopButton");
        _speedButton = GetUI<Button>("SpeedButton");
        _gameSpeedText = GetUI<TMPro.TMP_Text>("GameSpeedText");
        _monsterNameText = GetUI<TMPro.TMP_Text>("MonsterNameText");
        _waveText = GetUI<TMPro.TMP_Text>("WaveText");
        _timeText = GetUI<TMPro.TMP_Text>("TimeText");

        // Bottom Panel
        _curMonsterCountText = GetUI<TMPro.TMP_Text>("CurMonsterCountText");
        _jewelText = GetUI<TMPro.TMP_Text>("JewelText");
        _randomSppawnButton = GetUI<Button>("RandomSpawnButton");
        _unitSellButton = GetUI<Button>("SellUnitButton");

        // Unit Sell Panel
        _normalText = GetUI<TMPro.TMP_Text>("NormalText"); //TODO : 딕셔너리로 연결해서 수량 업데이트
        _rareText = GetUI<TMPro.TMP_Text>("RareText");
        _ancientText = GetUI<TMPro.TMP_Text>("AncientText");
        _legendText = GetUI<TMPro.TMP_Text>("LegendText");
        _epicText = GetUI<TMPro.TMP_Text>("EpicText");
        _closeSellUnitButton = GetUI<Button>("ClosePanelButton");
        _sellNornalButton = GetUI<Button>("SellNormalButton");
        _tribe1Button = GetUI<Button>("Tribe1Button"); //TODO : 버튼 클릭시 해당 부족 유닛만 보이게
        _tribe2Button = GetUI<Button>("Tribe2Button");
        _tribe3Button = GetUI<Button>("Tribe3Button");
    }

    private void Start()
    {
        // Top Panel
        AddPanelList(_warningPanel, _gameOverPanel);

        _stopButton.onClick.AddListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.AddListener(OnSpeedButtonClicked);

        _monsterSpawnmer.CurWaveChanged += OnWaveChanged;
        _waveManager.SpawnedMonsterCountChanged += OnCurMonsterCountChanged;

        // Bottom Panel
        GameManager.Instance.JewelChanged += OnJewelChanged;
        OnJewelChanged(GameManager.Instance.Jewel);

        _randomSppawnButton.onClick.AddListener(_randomUnitSpawner.SpawnRandomUnit);
        _unitSellButton.onClick.AddListener(() => _unitSellPanel.SetActive(true));

        // Unit Sell Panel
        _closeSellUnitButton.onClick.AddListener(() => _unitSellPanel.SetActive(false));
        /*_sellNornalButton.onClick.AddListener(() => _unitSpawnerController.SellUnit(_targetUnit));*/

        SetGameSpeedText();
    }

    private void OnDestroy()
    {
        // Top Panel
        _stopButton.onClick.RemoveListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.RemoveListener(OnSpeedButtonClicked);

        _monsterSpawnmer.CurWaveChanged -= OnWaveChanged;
        _waveManager.SpawnedMonsterCountChanged -= OnCurMonsterCountChanged;

        // Bottom Panel
        if (GameManager.Instance != null)
            GameManager.Instance.JewelChanged -= OnJewelChanged;

        _randomSppawnButton.onClick.RemoveListener(_randomUnitSpawner.SpawnRandomUnit);
        _unitSellButton.onClick.RemoveAllListeners();

        // Unit Sell Panel
        _closeSellUnitButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        _timeText.text = $"{System.TimeSpan.FromSeconds(GameManager.Instance.ClearTime):hh\\:mm\\:ss}";
    }

    private void OnSpeedButtonClicked()
    {
        GameManager.Instance.SetGameSpeed();
        SetGameSpeedText();
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

    public void OnCurMonsterCountChanged(int count)
    {
        Debug.Log($"OnCurMonsterCountChanged: {count}!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        _curMonsterCountText.text = $"{count}/1200";
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
}
