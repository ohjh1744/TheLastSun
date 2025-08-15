using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUpPanel : UIBInder
{
    //���� �ٿ������� �����ֱ� ���� UI
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
        //Updateüũ ������ DownLoadPanel�� ������ �׶� DownLoadüũ
        if (_updatePanel.activeSelf == false && _isCheckDownLoad == false)
        {
            DoCheckDownLoad();
        }
    }

    private void Init()
    {
        //�̸� ���� �Ҵ�
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        //��ư �Լ� ����
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
