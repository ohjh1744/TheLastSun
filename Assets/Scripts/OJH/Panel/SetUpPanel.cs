using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUpPanel : UIBInder
{
    //현재 다운진행을 보여주기 위한 UI
    private TextMeshProUGUI _downPercentText;
    private TextMeshProUGUI _downSizeText;
    private Slider _downPercentSlider;
    private Button _downLoadButton;

    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private GameObject _checkDownLoadPanel;
    [SerializeField] private GameObject _doDownLoadPanel;
    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private float _delayToFinishUpdate;
    [SerializeField] private float _delayToCheckDownLoad;
    [SerializeField] private float _delayToFinishDownLoad;


    private bool _isCheckDownLoad;


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
        if (_updatePanel.activeSelf == false && _isCheckDownLoad == false)
        {
            DoCheckDownLoad();
        }
    }

    private void Init()
    {
        //미리 변수 할당
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        //버튼 함수 연결
        _downLoadButton.onClick.AddListener(() => AddressableManager._instance.DoDownLoad(_downPercentSlider, _doDownLoadPanel, _mainPanel, _downPercentText, _delayToFinishDownLoad));

    }
    private void DoCheckUpdate()
    {
        GpgsManager.Instance.DoCheckForUpdate(_updatePanel, _checkDownLoadPanel, _delayToFinishUpdate);
    }
    private void DoCheckDownLoad()
    {
        _isCheckDownLoad = true;
        AddressableManager.Instance.DoCheckDownLoadFile(_downSizeText, _checkDownLoadPanel, _doDownLoadPanel, _mainPanel, _delayToCheckDownLoad);
    }





}
