using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIBInder
{
    [SerializeField] WaveManager _monsterSpawnmer;
    [SerializeField] WaveManager _waveManager;

    [HideInInspector] public GameObject _warningPanel;
    [HideInInspector] public GameObject _gameOverPanel;

    [Header("Top Panel")]
    private Button _stopButton;
    private Button _speedButton;
    private TMPro.TMP_Text _gameSpeedText;
    private TMPro.TMP_Text _monsterNameText;
    private TMPro.TMP_Text _waveText;
    private TMPro.TMP_Text _timeText;

    [Header("Bottom Panel")]
    private TMPro.TMP_Text _curMonsterCountText;

    private void Awake()
    {
        BindAll();
        InitUI();
    }

    private void InitUI()
    {
        _warningPanel = GetUI("WarningPanel");
        _gameOverPanel = GetUI("GameOverPanel");
        _stopButton = GetUI<Button>("StopButton");
        _speedButton = GetUI<Button>("SpeedButton");
        _gameSpeedText = GetUI<TMPro.TMP_Text>("GameSpeedText");
        _monsterNameText = GetUI<TMPro.TMP_Text>("MonsterNameText");
        _waveText = GetUI<TMPro.TMP_Text>("WaveText");
        _timeText = GetUI<TMPro.TMP_Text>("TimeText");
        _curMonsterCountText = GetUI<TMPro.TMP_Text>("CurMonsterCountText");
    }

    private void Start()
    {
        AddPanelList(_warningPanel, _gameOverPanel);

        _stopButton.onClick.AddListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.AddListener(OnSpeedButtonClicked);

        // 이벤트 구독
        _monsterSpawnmer.CurWaveChanged += OnWaveChanged;
        _waveManager.SpawnedMonsterCountChanged += OnCurMonsterCountChanged;

        SetGameSpeedText();
    }

    private void OnDestroy()
    {
        _stopButton.onClick.RemoveListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.RemoveListener(OnSpeedButtonClicked);

        _monsterSpawnmer.CurWaveChanged -= OnWaveChanged;
        _waveManager.SpawnedMonsterCountChanged -= OnCurMonsterCountChanged;
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
        _curMonsterCountText.text = $"{count}/1200";
    }

    private void AddPanelList(params GameObject[] panel)
    {
        foreach (var p in panel)
        {
            UIManager.Instance.Panels.Add(p);
        }
    }
}
