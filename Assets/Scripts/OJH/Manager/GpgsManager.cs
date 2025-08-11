using Google.Play.AppUpdate;
using Google.Play.Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GpgsManager : MonoBehaviour
{
    private  static GpgsManager _instance;
    public static GpgsManager Instance { get { return _instance; } set { _instance = value; } }

    private AppUpdateManager _appUpdateManager;

    Coroutine updateRoutine;

    [SerializeField] private float _delayToFinish;

    private WaitForSeconds _delayToFinishWs;

    private void Awake()
    {
        Debug.Log("GPGSManager Awake");
        if(_instance == null)
        {
            _instance = this;
            _delayToFinishWs = new WaitForSeconds(_delayToFinish);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void DoCheckForUpdate(GameObject nextPanel, TextMeshProUGUI nextText)
    {
        if(updateRoutine == null)
        {
            updateRoutine = StartCoroutine(CheckForUpdate(nextPanel, nextText));
        }
    }

    IEnumerator CheckForUpdate(GameObject nextPanel, TextMeshProUGUI nextText)
    {
        Debug.Log("Check!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //인앱 업데이트 관리를 위한 클래스 인스턴스화
        _appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            //업데이트 가능 상태라면 
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                var startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                //다운받기
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("업데이트 다운로드 진행중");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("다운르도 완료");
                    }
                    yield return null;
                }

                //실제 설치
                var result = _appUpdateManager.CompleteUpdate();

                //완료되었는지 마지막 확인
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            //업데이트가 없는 상태라면
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음!");

                yield return _delayToFinishWs;
                //로그인하기
                Login();
                //다음 패널 열어주기
                nextPanel.SetActive(true);
                nextText.text = "Check Resource";
            }
        }
        else
        {
            Debug.Log("업데이트 오류" + appUpdateInfoOperation.Error);
        }

        updateRoutine = null;
    }

    private void Login()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            string userID = PlayGamesPlatform.Instance.GetUserId();

            Debug.Log($"로그인 성공{displayName}{userID}");

        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }
}
