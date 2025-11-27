using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CreditPanel : UIBInder
{

    private void Awake()
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
        GetUI<Button>("CreditSetFalseButton").onClick.AddListener(SetFalsePanel);
    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }
}
