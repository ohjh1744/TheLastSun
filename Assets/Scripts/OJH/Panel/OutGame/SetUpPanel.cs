using Google.Play.AppUpdate;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetUpPanel : UIBInder
{
    //패널
    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private GameObject _checkDownLoadPanel;
    [SerializeField] private GameObject _doDownLoadPanel;
    [SerializeField] private GameObject _doDownLoadQuitGamePanel;

    //코루틴
    //Update확인 전에 Network연결을 확인하기 위한 주기
    [SerializeField] private float _checkCanUpdateRate;
    private Coroutine _checkUpdateRoutine;
    private WaitForSeconds _checkCanUpdateRateWs;

    private bool _isUpdateCheck;

    private void Awake()
    {
        BindAll();
        Init();
    }
    private void OnApplicationFocus(bool focus)
    {
        //앱이 처음 실행될때 or 업데이트창(focus == false)에서 다시 돌아올때 Update 잘 되어있는지 확인
        if (focus == true)
        {
            CheckUpdate();
        }
    }

    private void Start()
    {
        // 화면이 꺼지지 않도록 설정
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Login();
    }

    private void Update()
    {

        FailDownLoadForNetworkError();
    }

    private void Init()
    {
        //Ws 초기화
        _checkCanUpdateRateWs = new WaitForSeconds(_checkCanUpdateRate);
        //버튼 함수 연결
        GetUI<Button>("DownLoadButton").onClick.AddListener(() => DownLoad());
    }

    private void Login()
    {
        GpgsManager.Instance.Login();
    }

    //다운로드 가능한지 확인시작
    private void CheckUpdate()
    {
        if(_checkUpdateRoutine != null)
        {
            StopCoroutine(_checkUpdateRoutine);
        }
        _checkUpdateRoutine = StartCoroutine(OnCheckUpdate());
    }

    // 네트워크가 연결되어있는지 확인 후 연결되면 Update시작하고 종료
    IEnumerator OnCheckUpdate()
    {
        Debug.Log("업데이트 체크갑니다");
        while (true)
        {
            // 네트워크 연결되어있는 상태에서만
            if (NetworkCheckManager.Instance.IsConnected == true)
            {
                GpgsManager.Instance.CheckForUpdate((status =>
                {
                    //업데이트 할것이 있어, 창이 뜨는 순간에
                    if (status == UpdateAvailability.UpdateAvailable)
                    {
                        Debug.Log("업데이트 필요!");
                        string Url = "https://play.google.com/store/apps/details?id=com.OJH.TheLastSun";
                        Application.OpenURL(Url);
                    }
                    //업데이트 할것이 없다면
                    else if (status == UpdateAvailability.UpdateNotAvailable && _isUpdateCheck == false)
                    {
                        //간혹 정말 낮은 확률로 다운로드창까지 가서 _checkDownLoadPanel이 true가 되는 문제가 발생하여, 한번더 안전하게 해당 변수 추가
                        _isUpdateCheck = true;
                        // 현재 Panel인 UpdatePanel 닫고, 다음 Panel인 DownPanel 열기
                        _updatePanel.SetActive(false);
                        _checkDownLoadPanel.SetActive(true);
                        //Update 확인 끝나면 DownLoad 확인 시작
                        CheckDownLoad();
                    }
                }));
                break;
            }
            yield return _checkCanUpdateRateWs;
        }

        _checkUpdateRoutine = null;
    }

    //다운로드 할것이 있는지 확인
    private void CheckDownLoad()
    {
        Debug.Log("CheckDownLoad1");
        AddressableManager.Instance.DoCheckDownLoadFile((downSIze) =>{
            // 다운로드할 파일이 존재하면 다운로드 패널을 열기
            if (downSIze > decimal.Zero)
            {
                // CheckDownLoad 패널을 닫고, 다운로드 패널을 열기, 다운받을 용량 Text 내용 Update.
                _checkDownLoadPanel.SetActive(false);
                _doDownLoadPanel.SetActive(true);
                GetUI<TextMeshProUGUI>("DownSizeText").SetText(AddressableManager.Instance.GetFileSize(downSIze));
            }
            // 다운받을 파일이 존재하지 않으면 메인 패널을 열기
            else
            {
                Debug.Log("다운받을 파일이 없음!!!");
                // CheckDownLoad 패널을 닫고, 
                _checkDownLoadPanel.SetActive(false);
                //MainPanel씬으로 이동
                SceneManager.LoadScene(1);
            }
        });
    }

    //다운로드
    private void DownLoad()
    {
        // 다운로드 시작 하면 버튼 상호작용 끄기
        GetUI<Button>("DownLoadButton").interactable = false;

        //다운로드가 완전히 끝나면 
        AddressableManager._instance.DownLoad(GetUI<Slider>("DownPercentSlider"), GetUI<TextMeshProUGUI>("DownPercentText"), (isDownFinish) =>
        {
            if (isDownFinish == true)
            {
                //MainPanel씬으로 이동
                SceneManager.LoadScene(1);
            }
        });
    }

    //다운로드 중에 네트워크 오류 시 QuitGamePanel 띄우기
    private void FailDownLoadForNetworkError()
    {
        if(NetworkCheckManager.Instance.IsConnected == false && _doDownLoadPanel.activeSelf == true)
        {
            //다운로드 버튼 비활성화
            GetUI<Button>("DownLoadButton").interactable = false;
            _doDownLoadQuitGamePanel.SetActive(true);
        }
    }

}
