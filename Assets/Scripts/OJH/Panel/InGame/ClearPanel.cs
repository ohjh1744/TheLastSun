using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearPanel : UIBInder
{
    StringBuilder _sb = new StringBuilder();


    void Awake()
    {
        BindAll();
    }
    void Start()
    {
        Init();
        AddEvent();
    }

    private void Init()
    {
        //클리어 여부
        _sb.Clear();
        if (InGameManager.Instance.GameState == EGameState.Win)
        {
            _sb.Append("클리어 성공");
        }
        else
        {
            _sb.Append("클리어 실패");
        }
        GetUI<TextMeshProUGUI>("ClearPanelNameBgText").SetText(_sb);


        //웨이브숫자
        _sb.Clear();
        _sb.Append($"Wave {InGameManager.Instance.WaveNum}");
        GetUI<TextMeshProUGUI>("ClearPanelWaveInfoText").SetText(_sb);

        //플레이타임
        string formattedTime = TimeSpan.FromSeconds(InGameManager.Instance.PlayTime).ToString(@"hh\:mm\:ss");
        _sb.Clear();
        _sb.Append(formattedTime);
        GetUI<TextMeshProUGUI>("ClearPanelTimerInfoText").SetText(_sb);

    }
    private void AddEvent()
    {
        GetUI<Button>("ClearPanelGoLobbyButton").onClick.AddListener(GoLobby);
    }

    private void GoLobby()
    {
        SceneManager.LoadScene(1);
    }

}
