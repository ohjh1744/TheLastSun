using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUpPanel : UIBInder
{
    //현재 다운진행을 보여주기 위한 UI
    private TextMeshProUGUI _doWhatText;
    private TextMeshProUGUI _downPercentText;
    private TextMeshProUGUI _downSizeText;
    private Slider _downPercentSlider;
    private Button _downLoadButton;

    [SerializeField] private GameObject _downLoadPanel;
    [SerializeField] private GameObject _mainPanel;


    private void Awake()
    {
        BindAll();
        Init();
    }
    private void Start()
    {
        DoCheckUpdate();
    }

    private void Update()
    {
        //Update체크 끝난후 DownLoadPanel이 나오면 그때 DownLoad체크
        if (_downLoadPanel.activeSelf == true)
        {
            DoCheckDownLoad();
        }
    }

    private void Init()
    {
        //미리 변수 할당
        _doWhatText = GetUI<TextMeshProUGUI>("DoWhatText");
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        //버튼 함수 연결
        _downLoadButton.onClick.AddListener(() => AddressableManager._instance.DoDownLoad(_downPercentSlider, _mainPanel, _downPercentText));

    }
    private void DoCheckUpdate()
    {
        GpgsManager.Instance.DoCheckForUpdate(_downLoadPanel, _doWhatText);
    }
    private void DoCheckDownLoad()
    {
        AddressableManager.Instance.DoCheckDownLoadFile(_downSizeText, _downPercentText, _downPercentSlider, _downLoadButton, _mainPanel);
    }





}
