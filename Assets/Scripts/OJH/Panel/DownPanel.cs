using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownPanel : UIBInder
{
    //현재 다운진행을 보여주기 위한 UI
    [SerializeField] private GameObject _mainPanel;
    private TextMeshProUGUI _downPercentText;
    private TextMeshProUGUI _downSizeText;
    private Slider _downPercentSlider;
    private Button _downLoadButton;

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        _downLoadButton.onClick.AddListener(() => AddressableManager._instance.DoDownLoad(_downPercentSlider, _mainPanel, _downPercentText));
        Debug.Log(AddressableManager.Instance);
        AddressableManager.Instance.DoCheckDownLoadFile(_downSizeText, _downPercentText, _downPercentSlider, _downLoadButton, _mainPanel);
    }

    
}
