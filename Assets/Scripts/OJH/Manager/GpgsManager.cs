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


    // GPGS Ŭ���忡 ������ ������ ���� �̸�
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

    // currentPanel�� UpdatePanel
    // nextPanel�� CheckDownLoadPanel
    // DoCheckForUpdate �޼��忡�� �ݹ��� ���� UpdateAvailability ���� ��ȯ
    public void DoCheckForUpdate(Action<UpdateAvailability> callback)
    {
        if (_updateRoutine == null)
        {
            _updateRoutine = StartCoroutine(CheckForUpdate(callback));
        }
    }

    // CheckForUpdate �ڷ�ƾ���� UpdateAvailability ���� ����ϰ� �ݹ� ȣ��
    IEnumerator CheckForUpdate( Action<UpdateAvailability> callback)
    {
        yield return _delayToStartUpdateWs;

        Debug.Log("Check for update...");

        // �ξ� ������Ʈ ������ ���� Ŭ���� �ν��Ͻ�ȭ
        _appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            // ������Ʈ ���� ���¶��
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                // �ٿ�ε� ����
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("������Ʈ �ٿ�ε� ������");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("�ٿ�ε� �Ϸ�");
                    }
                    yield return null;
                }

                // �ٿ�ε� �Ϸ� �� ������Ʈ ���� ����
                var result = _appUpdateManager.CompleteUpdate();

                // �Ϸ�Ǿ����� ������ Ȯ��
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                Debug.Log("������Ʈ �Ϸ�");

                yield return (int)startUpdateRequest.Status;

                // ������Ʈ ���¸� �ݹ����� ��ȯ
                callback(UpdateAvailability.UpdateAvailable);
            }
            // ������Ʈ�� ���� ���¶��
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("������Ʈ ����!");

                yield return _delayToFinishUpdateWs;

                // �ݹ����� UpdateNotAvailable ���¸� ��ȯ
                callback(UpdateAvailability.UpdateNotAvailable);
            }
        }
        else
        {
            Debug.Log("������Ʈ ����: " + appUpdateInfoOperation.Error);
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

            Debug.Log($"�α��� ����{displayName}{userID}");

            //�α��� ������ ���������� ��������
            LoadData((status) => {});
        }
        else
        {
            Debug.Log("�α��� ����");
        }
    }

    //Ŭ���忡 Data�����ϱ�
    public void SaveData(Action<SavedGameRequestStatus> callback)
    {
        // Ŭ���� ����ҿ� ��ȣ�ۿ� ������ �������̽�
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        // ����� ���� ������ ������ ���� �ݹ� �Լ� ����
        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("Save ���� ����");

                var update = new SavedGameMetadataUpdate.Builder().Build();

                // Player �����͸� JSON���� ����ȭ
                var json = JsonUtility.ToJson(PlayerController.Instance.PlayerData);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                // ���� �����͸� Ŀ��(����)
                savedGameClient.CommitUpdate(game, update, bytes, (writeStatus, writtenGame) =>
                {
                    if (writeStatus == SavedGameRequestStatus.Success)
                    {
                        // ���� ���� �ݹ� ȣ��
                        Debug.Log("���� ����");
                        callback(SavedGameRequestStatus.Success);
                    }
                    else
                    {
                        // ���� ���� �ݹ� ȣ��
                        Debug.Log("���� ����");
                        callback(SavedGameRequestStatus.InternalError);
                    }
                });
            }
            else
            {
                // Save ���� ���� �ݹ� ȣ��
                Debug.Log("Save ���� ����");
                callback(SavedGameRequestStatus.InternalError);
            }
        });
    }


    // LoadData �Լ����� �ݹ��� ����Ͽ� ����� ��ȯ
    public void LoadData(Action<SavedGameRequestStatus> callback)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood, (status, data) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    Debug.Log("Load ���� ����");

                    // ������ ���������� �������� �����͸� �б�
                    savedGameClient.ReadBinaryData(data, (readStatus, loadedData) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            // ������ �б� ����
                            string json = Encoding.UTF8.GetString(loadedData);

                            if (string.IsNullOrEmpty(json))
                            {
                                Debug.Log("����� �����Ͱ� ����");
                            }
                            else
                            {
                                Debug.Log($"Load Read Data: {json}");

                                // JSON �����͸� PlayerData ��ü�� ��ȯ
                                PlayerController.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(json);
                            }

                            // ������ �б� ����� �ݹ����� ��ȯ
                            callback(SavedGameRequestStatus.Success);
                        }
                        else
                        {
                            Debug.Log("������ �б� ����");
                            callback(SavedGameRequestStatus.InternalError);
                        }
                    });
                }
                else
                {
                    // ���� ���� ���� �ݹ� ȣ��
                    Debug.Log("Load ���� ����");
                    callback(SavedGameRequestStatus.InternalError);
                }
            });
    }

    // DeleteData �Լ����� �ݹ��� ����Ͽ� ����� ��ȯ
    public void DeleteData(Action<SavedGameRequestStatus> callback)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood, (status, data) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    // ������ ���������� �������� �����͸� ����
                    savedGameClient.Delete(data);

                    Debug.Log("������ ���� ����");

                    // �÷��̾� ���� �ʱ�ȭ
                    PlayerController.Instance.SetClear();

                    // ���� ���� �ݹ� ȣ��
                    callback(SavedGameRequestStatus.Success);
                }
                else
                {
                    // ���� ���� ���� �ݹ� ȣ��
                    Debug.Log("������ ���� ����");
                    callback(SavedGameRequestStatus.InternalError);
                }
            });
    }


    //��� �������� UI ǥ��
    public void ShowAllLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    //�ð� ��ŷ�� ����
    // gpgs���� �ð������� ms�� 1000ms�� 1�ʸ� �ǹ�
    public void UpdateTimeLeaderboard( float time, string leaderboardID)
    {
        //float���� long���� ��ȯ
        // ex) 1.23f -> 1230ms(long)
        long scoreToReport = (long)(time * 1000);

        //Num ���� �������� ������Ʈ
        PlayGamesPlatform.Instance.ReportScore(scoreToReport, leaderboardID, (bool success) => { Debug.Log("AddTime" + time); });
    }


}
