using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : UIBInder
{
    [Header("Top Panel")]
    private Button _stopButton;
    private Button _speedButton;
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
        // Top Panel
        _stopButton = GetUI("StopButton").GetComponent<Button>();
        _speedButton = GetUI("SpeedButton").GetComponent<Button>();
        _monsterNameText = GetUI("MonsterNameText").GetComponent<TMPro.TMP_Text>();
        _waveText = GetUI("WaveText").GetComponent<TMPro.TMP_Text>();
        _timeText = GetUI("TimeText").GetComponent<TMPro.TMP_Text>();

        // Bottom Panel
        _curMonsterCountText = GetUI("CurMonsterCountText").GetComponent<TMPro.TMP_Text>();
    }

    private void OnEnable()
    {
        _stopButton.clicked += () => GameManager.Instance.StopTimer();
    }

    private void Update()
    {
        _timeText.text = $"Time : {System.TimeSpan.FromSeconds(GameManager.Instance.ClearTime):hh\\:mm\\:ss}";
    }
}
