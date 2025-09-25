using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailDownLoadPanel : UIBInder
{
    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        AddEvent();
    }

    private void AddEvent()
    {
        GetUI<Button>("FailDownLoadForNetworkErrorPanelButton").onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        Debug.Log("Á¾·á");
        Application.Quit();
    }
}
