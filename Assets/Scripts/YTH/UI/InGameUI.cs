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

        AddPanelList(_warningPanel, _gameOverPanel);
    }

    private void InitUI()
    {
        _warningPanel = GetUI("WarningPanel");
        _gameOverPanel = GetUI("GameOverPanel");

        // Top Panel
        _stopButton = GetUI<Button>("StopButton");
        _speedButton = GetUI<Button>("SpeedButton");
        _gameSpeedText = GetUI<TMPro.TMP_Text>("GameSpeedText");
        _monsterNameText = GetUI<TMPro.TMP_Text>("MonsterNameText");
        _waveText = GetUI<TMPro.TMP_Text>("WaveText");
        _timeText = GetUI<TMPro.TMP_Text>("TimeText");

        // Bottom Panel
        _curMonsterCountText = GetUI<TMPro.TMP_Text>("CurMonsterCountText");
    }

    private void Start()
    {
        _stopButton.onClick.AddListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.AddListener(OnSpeedButtonClicked);
        _monsterSpawnmer.CurWave.Subscribe(OnWaveChanged);

        SetGameSpeedText();
    }

    private void OnDestroy()
    {
        _stopButton.onClick.RemoveListener(GameManager.Instance.PauseGame);
        _speedButton.onClick.RemoveListener(OnSpeedButtonClicked);
        _monsterSpawnmer.CurWave.Unsubscribe(OnWaveChanged);
    }

    private void Update()
    {
        _timeText.text = $"Time : {System.TimeSpan.FromSeconds(GameManager.Instance.ClearTime):hh\\:mm\\:ss}";
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
    private void OnWaveChanged(int wave)
    {
        _waveText.text = $"Wave {wave}";
    }

    private void OnCurMonsterCountTextChangerd()
    {
       // 수정할것
       /* _waveManager._spawnedMonsterCount.*/
    }

    /// <summary>
    /// UIManager의 Panels 리스트에 패널 추가
    /// </summary>
    /// <param name="panel"></param>
    private void AddPanelList(params GameObject[] panel)
    {
        foreach (var p in panel)
        {   
            UIManager.Instance.Panels.Add(p);
        }
    }
}
