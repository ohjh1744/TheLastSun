using System.Collections;
using System.Collections.Generic;
using System.Net;
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

    [SerializeField] private float _delayToStartUpdate;
    [SerializeField] private float _delayToFinishUpdate;
    [SerializeField] private float _delayToStartDownLoad;
    [SerializeField] private float _delayToFinishDownLoad;

    private Coroutine _routine;
    private WaitForSeconds _checkCanUpdateRateWs;
    [SerializeField] private float _checkCanUpdateRate;

    private bool _isCheckDownLoad;


    private void Awake()
    {
        BindAll();
        Init();
    }
    private void Start()
    {
        DoLogin();
        DoCheckUpdate();
    }
    private void Update()
    {
        // 네트워크연결되어 있는 경우만 아래 동작 가능하도록 
        if(NetworkCheckManager.Instance.IsConnected == false)
        {
            //네트워크 연결이 안되면 모든 버튼 못누르도록
            SetInteractableFalse();
            return;
        }

        SetInteractableTrue();

        //Update체크 끝난후 DownLoadPanel이 나오면 그때 DownLoad체크
        if (_updatePanel.activeSelf == false && _isCheckDownLoad == false)
        {
            DoCheckDownLoad();
        }

    }

    private void Init()
    {
        //Ws 초기화
        _checkCanUpdateRateWs = new WaitForSeconds(_checkCanUpdateRate);

        //미리 변수 할당
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        //버튼 함수 연결
        _downLoadButton.onClick.AddListener(() => AddressableManager._instance.DoDownLoad(_downPercentSlider, _doDownLoadPanel, _mainPanel, _downPercentText, _delayToFinishDownLoad));

    }

    //네트워크 문제시 모든 버튼 비활성화
    private void SetInteractableFalse()
    {
        _downLoadButton.interactable = false;
    }

    //네트워크 연결되면 모든 버튼 활성화
    private void SetInteractableTrue()
    {
        _downLoadButton.interactable = true;
    }

    private void DoLogin()
    {
        GpgsManager.Instance.Login();
    }

    //다운로드 가능한지 확인시작
    private void DoCheckUpdate()
    {
        if(_routine == null)
        {
            _routine = StartCoroutine(CheckUpdate());
        }
    }

    // 네트워크가 연결되어있는지 확인 후 연결되면 Update시작하고 종료
    IEnumerator CheckUpdate()
    {
        while (true)
        {
            // 네트워크 연결되어있는 상태에서만
            if (NetworkCheckManager.Instance.IsConnected == true)
            {
                GpgsManager.Instance.DoCheckForUpdate(_updatePanel, _checkDownLoadPanel, _delayToStartUpdate, _delayToFinishUpdate);
                break;
            }
            yield return _checkCanUpdateRate;
        }

        _routine = null;
    }
    private void DoCheckDownLoad()
    {
        _isCheckDownLoad = true;
        AddressableManager.Instance.DoCheckDownLoadFile(_downSizeText, _checkDownLoadPanel, _doDownLoadPanel, _mainPanel, _delayToStartDownLoad);
    }





}
