using Google.Play.AppUpdate;
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
    private TextMeshProUGUI _updateDetailText;
    private Slider _downPercentSlider;
    private Button _downLoadButton;

    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private GameObject _checkDownLoadPanel;
    [SerializeField] private GameObject _doDownLoadPanel;
    [SerializeField] private GameObject _mainPanel;

    private Coroutine _routine;
    //Update확인 전에 Network연결을 확인하기 위한 주기
    private WaitForSeconds _checkCanUpdateRateWs;
    [SerializeField] private float _checkCanUpdateRate;


    private Coroutine _routine2;

    private bool _isUpdating;

    private void Awake()
    {
        BindAll();
        Init();
    }
    private void Start()
    {
        DoLogin();
    }

    private void OnApplicationFocus(bool focus)
    {
        //업데이크 체크가 완료되지않았따면 업데이크 확인 시작
        if(focus == true && _isUpdating == false)
        {
            DoCheckUpdate();
        }

        //업데이트 중에 뒤로가기를 했다면
        if (focus == true && _isUpdating == true)
        {
            DoQuitWhenBackInUpdate();
        }
    }
    private void Update()
    {
        // 네트워크연결되어 있지 않는 경우
        if(NetworkCheckManager.Instance.IsConnected == false)
        {
            // true로 해논 이유는 
            // 네트워크 팝업창이 뜨게 되면 다운버튼이 뒤로 숨겨지는데
            // 다운로드 상황에서 네트워크가 해지되었다가 다시 연결되면, 다운로드를 다시 할수있도록 하기위해서
            _downLoadButton.interactable = true;
            return;
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
        _updateDetailText = GetUI<TextMeshProUGUI>("UpdateDetailText");

        //버튼 함수 연결
        _downLoadButton.onClick.AddListener(() => DoDownLoad());

    }

    private void DoLogin()
    {
        GpgsManager.Instance.Login();
    }

    //다운로드 가능한지 확인시작
    private void DoCheckUpdate()
    {
        if(_routine != null)
        {
            StopCoroutine(_routine);
        }
        _routine = StartCoroutine(CheckUpdate());
    }

    // 네트워크가 연결되어있는지 확인 후 연결되면 Update시작하고 종료
    IEnumerator CheckUpdate()
    {
        Debug.Log("업데이트 체크갑니다");
        while (true)
        {
            // 네트워크 연결되어있는 상태에서만
            if (NetworkCheckManager.Instance.IsConnected == true)
            {
                GpgsManager.Instance.DoCheckForUpdate((status =>
                {
                    //업데이트 할것이 있어, 창이 뜨는 순간에 리턴됨.
                    if (status == UpdateAvailability.UpdateAvailable)
                    {
                        Debug.Log("업데이트 텍스트 변환!");
                        _isUpdating = true;
                        Debug.Log($"isUpdating: {_isUpdating}");
                    }
                    //업데이트 할것이 없다면 리턴 됨
                    else if (status == UpdateAvailability.UpdateNotAvailable)
                    {
                        // 현재 Panel인 UpdatePanel 닫고, 다음 Panel인 DownPanel 열기
                        _updatePanel?.SetActive(false);
                        _checkDownLoadPanel?.SetActive(true);
                        //Update 확인 끝나면 DownLoad 확인 시작
                        DoCheckDownLoad();
                    }
                }));
                break;
            }
            yield return _checkCanUpdateRateWs;
        }

        _routine = null;
    }

    //업데이트 하는 도중에 뒤로갔을때 종료하기
    private void DoQuitWhenBackInUpdate()
    {
        if(_routine2 == null)
        {
            _routine2 = StartCoroutine(QuitWhenBackInUpdate());
        }
    }

    IEnumerator QuitWhenBackInUpdate()
    {
        _updateDetailText.text = "업데이트에 실패하여 앱이 종료 됩니다.";
        yield return new WaitForSeconds(2f);

        Application.Quit();
        _routine2 = null;
    }

    private void DoCheckDownLoad()
    {
        AddressableManager.Instance.DoCheckDownLoadFile((downSIze) =>{
            // 다운로드할 파일이 존재하면 다운로드 패널을 열기
            if (downSIze > decimal.Zero)
            {
                // CheckDownLoad 패널을 닫고, 다운로드 패널을 열기, 다운받을 용량 Text 내용 Update.
                _checkDownLoadPanel.SetActive(false);
                _doDownLoadPanel.SetActive(true);
                _downSizeText.SetText(AddressableManager.Instance.GetFileSize(downSIze));
            }
            // 다운받을 파일이 존재하지 않으면 메인 패널을 열기
            else
            {
                // CheckDownLoad 패널을 닫고, 바로 메인 패널을 열기
                _checkDownLoadPanel.SetActive(false);
                _mainPanel.SetActive(true);
                Debug.Log("다운받을 파일이 없음!!!");
            }
        });
    }

    private void DoDownLoad()
    {
        // 다운로드 시작 하면 버튼 상호작용 끄기
        _downLoadButton.interactable = false;

        //다운로드가 완전히 끝나면 
        AddressableManager._instance.DoDownLoad(_downPercentSlider, _downPercentText, (isDownFinish) =>
        {
            if (isDownFinish == true)
            {
                //DoDownLoadPanel 켜주기
                _doDownLoadPanel.SetActive(false);
                _mainPanel.SetActive(true);
            }
        });
    }





}
