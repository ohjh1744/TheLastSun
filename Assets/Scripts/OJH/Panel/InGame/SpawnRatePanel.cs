using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
public class SpawnRatePanel : UIBInder
{
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
        GetUI<Button>("SpawnRatePanelSetFalseButton").onClick.AddListener(SetFalsePanel);
    }


    //패널 끄면서 다시 Play로 바꿔주기
    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
        
    }
}
