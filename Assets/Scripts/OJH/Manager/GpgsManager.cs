using Google.Play.AppUpdate;
using Google.Play.Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
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

    WaitForSeconds _delayToStartUpdateWs;

    WaitForSeconds _delayToFinishUpdateWs;

    [SerializeField] private float _delayToStartUpdate;

    [SerializeField] private float _delayToFinishUpdate;


    // GPGS 클라우드에 저장할 데이터 파일 이름
    private static string _saveFileName = "file.dat";


    private void Awake()
    {

        if(_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        _delayToStartUpdateWs = new WaitForSeconds(_delayToStartUpdate);
        _delayToFinishUpdateWs = new WaitForSeconds(_delayToFinishUpdate);
    }

    // currentPanel은 UpdatePanel
    // nextPanel은 CheckDownLoadPanel
    // DoCheckForUpdate 메서드에서 콜백을 통해 UpdateAvailability 값을 반환
    public void CheckForUpdate(Action<UpdateAvailability> callback)
    {
        if (_updateRoutine != null)
        {
            StopCoroutine(_updateRoutine);
        }
        _updateRoutine = StartCoroutine(OnCheckForUpdate(callback));

    }

    // CheckForUpdate 코루틴에서 UpdateAvailability 값을 계산하고 콜백 호출
    IEnumerator OnCheckForUpdate(Action<UpdateAvailability> callback)
    {
        yield return _delayToStartUpdateWs;

        Debug.Log("Check for update...");

        // 인앱 업데이트 관리를 위한 클래스 인스턴스화
        _appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            // 업데이트 가능 상태 or 이전에 업데이트를 했으나 완료되지 않은 상태 -> 업데이트 중 앱을 끄거나 중간에 문제가 생겨 앱이 다시 시작되었을때 발생.
            // 구글에선  후자의 경우 이때 StartUpdate를 다시 호출하도록 유도하라고함.
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable || appUpdateInfoResult.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                //테스트를 위해서 주석 추가
                if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
                {
                    Debug.Log("업데이트 전에 진행한적 있음");
                }
                callback(UpdateAvailability.UpdateAvailable);

                //실제 업데이트 창 띄우기
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                // 다운로드 진행
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

                // 다운로드 완료 후 업데이트 실제 적용
                var result = _appUpdateManager.CompleteUpdate();

                // 완료되었는지 마지막 확인
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                Debug.Log("업데이트 완료");

                yield return (int)startUpdateRequest.Status;
            }
            // 업데이트가 없는 상태라면
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음!");

                yield return _delayToFinishUpdateWs;

                // 콜백으로 UpdateNotAvailable 상태를 반환
                callback(UpdateAvailability.UpdateNotAvailable);
            }
        }
        else
        {
            Debug.Log("업데이트 오류: " + appUpdateInfoOperation.Error);
        }

        _updateRoutine = null;
    }



    public void Login()
    {
        PlayGamesPlatform.Instance.Authenticate(OnProcessAuthentication);
    }

    internal void OnProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            string userID = PlayGamesPlatform.Instance.GetUserId();

            Debug.Log($"로그인 성공{displayName}{userID}");

            //로그인 성공후 유저데이터 가져오기
            LoadData((status) => {});
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }

    //클라우드에 Data저장하기
    public void SaveData(Action<SavedGameRequestStatus> callback)
    {
        // 클라우드 저장소와 상호작용 가능한 인터페이스
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        // 저장된 게임 데이터 파일을 열고 콜백 함수 지정
        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("Save 열기 성공");

                var update = new SavedGameMetadataUpdate.Builder().Build();

                // Player 데이터를 JSON으로 직렬화
                var json = JsonUtility.ToJson(PlayerController.Instance.PlayerData);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                // 게임 데이터를 커밋(저장)
                savedGameClient.CommitUpdate(game, update, bytes, (writeStatus, writtenGame) =>
                {
                    if (writeStatus == SavedGameRequestStatus.Success)
                    {
                        // 저장 성공 콜백 호출
                        Debug.Log("저장 성공");
                        callback(SavedGameRequestStatus.Success);
                    }
                    else
                    {
                        // 저장 실패 콜백 호출
                        Debug.Log("저장 실패");
                        callback(SavedGameRequestStatus.InternalError);
                    }
                });
            }
            else
            {
                // Save 열기 실패 콜백 호출
                Debug.Log("Save 열기 실패");
                callback(SavedGameRequestStatus.InternalError);
            }
        });
    }


    // LoadData 함수에서 콜백을 사용하여 결과를 반환
    public void LoadData(Action<SavedGameRequestStatus> callback)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly,
            ConflictResolutionStrategy.UseLastKnownGood, (status, data) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    Debug.Log("Load 열기 성공");

                    // 파일을 성공적으로 열었으면 데이터를 읽기
                    savedGameClient.ReadBinaryData(data, (readStatus, loadedData) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            // 데이터 읽기 성공
                            string json = Encoding.UTF8.GetString(loadedData);

                            if (string.IsNullOrEmpty(json))
                            {
                                Debug.Log("저장된 데이터가 없음");
                            }
                            else
                            {
                                Debug.Log($"Load Read Data: {json}");

                                // JSON 데이터를 PlayerData 객체로 변환
                                PlayerController.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(json);

                                Debug.Log($"Load Player Data: {JsonUtility.ToJson(PlayerController.Instance.PlayerData)}");
                            }

                            // 데이터 읽기 결과를 콜백으로 반환
                            callback(SavedGameRequestStatus.Success);
                        }
                        else
                        {
                            Debug.Log("데이터 읽기 실패");
                            callback(SavedGameRequestStatus.InternalError);
                        }
                    });
                }
                else
                {
                    // 파일 열기 실패 콜백 호출
                    Debug.Log("Load 열기 실패");
                    callback(SavedGameRequestStatus.InternalError);
                }
            });
    }

    // DeleteData 함수에서 콜백을 사용하여 결과를 반환
    public void DeleteData(Action<SavedGameRequestStatus> callback)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly,
            ConflictResolutionStrategy.UseLastKnownGood, (status, data) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    // 파일을 성공적으로 열었으면 데이터를 삭제
                    savedGameClient.Delete(data);

                    Debug.Log("데이터 삭제 성공");

                    // 플레이어 상태 초기화
                    PlayerController.Instance.SetClear();

                    // 삭제 성공 콜백 호출
                    callback(SavedGameRequestStatus.Success);
                }
                else
                {
                    // 파일 열기 실패 콜백 호출
                    Debug.Log("데이터 삭제 실패");
                    callback(SavedGameRequestStatus.InternalError);
                }
            });
    }


    //모든 리더보드 UI 표시
    public void ShowAllLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    //시간 랭킹에 갱신
    // gpgs에서 시간단위는 ms로 1000ms면 1초를 의미
    public void UpdateTimeLeaderboard( float time, string leaderboardID, Action<bool> callback)
    {
        //float에서 long으로 변환
        // ex) 1.23f -> 1230ms(long)
        long scoreToReport = (long)(time * 1000);

        //Num 형식 리더보드 업데이트
        PlayGamesPlatform.Instance.ReportScore(scoreToReport, leaderboardID, (bool success) =>
        {
            if (success == true)
            {
                Debug.Log($"리더보드 업데이트 성공: {time}s ({scoreToReport}ms)");
                callback(success);
            }
            else
            {
                Debug.LogError("리더보드 업데이트 실패!");
                callback(success);
            }
        });
    }


}
