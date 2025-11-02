using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : UIBInder
{
    [SerializeField] private GameObject _tutorialPanel;
    void Awake()
    {
        BindAll();
    }
    void Start()
    {
        Init();
    }

    private void Init()
    {
        AddEvent();
    }

    private void AddEvent()
    {
        GetUI<Button>("GoTutorialButton").onClick.AddListener(GoTutorial);
        GetUI<Button>("PausePanelGoLobbyButton").onClick.AddListener(GoLobby);
        GetUI<Button>("PausePanelSetFalseButton").onClick.AddListener(SetFalsePanel);
    }

    private void GoTutorial()
    {
        _tutorialPanel.SetActive(true);
    }

    private void GoLobby()
    {
        SceneManager.LoadScene(1);
    }

    //패널 끄면서 다시 Play로 바꿔주기
    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
        InGameManager.Instance.GameState = EGameState.Play;
    }



}
