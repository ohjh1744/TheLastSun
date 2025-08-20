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

    Coroutine _updateRoutine;

    // GPGS 클라우드에 저장할 데이터 파일 이름
    private static string _saveFileName = "file.dat";


    private bool _isCheckUpdate = false;
    public bool isCheckUpdate {get { return _isCheckUpdate; } set { _isCheckUpdate = value; } }

    private void Awake()
    {
        Debug.Log("GPGSManager Awake");
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // currentPanel은 UpdatePanel
    // nextPanel은 CheckDownLoadPanel
    public void DoCheckForUpdate(GameObject updatePanel, GameObject checkDownLoadPanel, float delayToStartCurrentWork, float delayToFinishCurrentWork)
    {
        if(_updateRoutine == null)
        {
            _updateRoutine = StartCoroutine(CheckForUpdate(updatePanel, checkDownLoadPanel, delayToStartCurrentWork, delayToFinishCurrentWork));
        }
    }

    IEnumerator CheckForUpdate(GameObject updatePanel, GameObject checkDownLoadPanel, float delayToStartCurrentWork, float delayToFinishCurrentWork)
    {
        yield return new WaitForSeconds(delayToStartCurrentWork);

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
                        Debug.Log("다운로드 완료");
                    }
                    yield return null;
                }

                //다운로드 완료후 업데이트를 실제로 적용.
                var result = _appUpdateManager.CompleteUpdate();

                //완료되었는지 마지막 확인
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                Debug.Log("업데이트 완료");
                _isCheckUpdate = true;
                yield return (int)startUpdateRequest.Status;

            }
            //업데이트가 없는 상태라면
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음!");

                yield return new WaitForSeconds(delayToFinishCurrentWork);

                //현재 Panel인 UpdatePanel 닫고, 다음 Panel인 DownPanel 열기
                updatePanel?.SetActive(false);
                checkDownLoadPanel?.SetActive(true);
                _isCheckUpdate = true;
            }
        }
        else
        {
            Debug.Log("업데이트 오류" + appUpdateInfoOperation.Error);
        }

        _updateRoutine = null;
    }

    public void Login()
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

            //로그인 성공후 유저데이터 가져오기
            LoadData();

        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }

    //클라우드에 Data저장하기
    public void SaveData()
    {
        //클라우드 저장소와 상호작용 가능한 인터페이스
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        // 1번째 인자 파일이름, 2번째 네트워크 연결된 상태에서만 Data 저장및 불러오기 가능
        // 3번째 인자 마지막에 정상적으로 저장된 정보를 가져옴, 4번째 인자 콜백 함수
        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLastKnownGood, OnSaveDataOpend);
    }

    //저장된 게임 데이터에 대한 요청 결과 상태를 다룬 인터페이스, 저장된 게임의 메타 데이터를 다룬 인터페이스
    private void OnSaveDataOpend(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Save열기 성공");

            var update = new SavedGameMetadataUpdate.Builder().Build();

            //json
            var json = JsonUtility.ToJson(PlayerController.Instance.PlayerData);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            Debug.Log($"저장 데이터: {bytes}");

            savedGameClient.CommitUpdate(game, update, bytes, OnSaveDataWritten);


        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("Save열기 실패");
            Debug.Log($"{status}");
        }

    }

    private void OnSaveDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("저장 성공");
        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("저장 실패");
        }
    }

    //클라우드에서 Data 가져오기
    public void LoadData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, OnLoadDataOpend);
    }

    private void OnLoadDataOpend(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Load 열기 성공");

            savedGameClient.ReadBinaryData(data, OnLoadDataRead);
        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("Load 열기 실패");
        }
    }

    private void OnLoadDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        string data = Encoding.UTF8.GetString(loadedData);

        if (data == "")
        {
            Debug.Log("저장된 데이터가 없음");
        }
        else
        {
            Debug.Log($"Load Read Data: {data}");

            //json
            PlayerController.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(data);
        }
    }

    //Data 삭제
    public void DeleteData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, OnDeleteSaveData);
    }

    private void OnDeleteSaveData(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            savedGameClient.Delete(data);
            Debug.Log("데이터 삭제 성공");
            PlayerController.Instance.SetClear();
        }
        else
        {
            Debug.Log("데이터 삭제 실패");
        }
    }


    //모든 리더보드 UI 표시
    public void ShowAllLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    //시간 랭킹에 갱신
    // gpgs에서 시간단위는 ms로 1000ms면 1초를 의미
    public void UpdateTimeLeaderboard( float time, string leaderboardID)
    {
        //float에서 long으로 변환
        // ex) 1.23f -> 1230ms(long)
        long scoreToReport = (long)(time * 1000);

        //Num 형식 리더보드 업데이트
        PlayGamesPlatform.Instance.ReportScore(scoreToReport, leaderboardID, (bool success) => { Debug.Log("AddTime" + time); });
    }


}
