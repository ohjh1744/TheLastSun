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

    // GPGS Ŭ���忡 ������ ������ ���� �̸�
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

    // currentPanel�� UpdatePanel
    // nextPanel�� CheckDownLoadPanel
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
        //�ξ� ������Ʈ ������ ���� Ŭ���� �ν��Ͻ�ȭ
        _appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            //������Ʈ ���� ���¶�� 
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                //�ٿ�ޱ�
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

                //�ٿ�ε� �Ϸ��� ������Ʈ�� ������ ����.
                var result = _appUpdateManager.CompleteUpdate();

                //�Ϸ�Ǿ����� ������ Ȯ��
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                Debug.Log("������Ʈ �Ϸ�");
                _isCheckUpdate = true;
                yield return (int)startUpdateRequest.Status;

            }
            //������Ʈ�� ���� ���¶��
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("������Ʈ ����!");

                yield return new WaitForSeconds(delayToFinishCurrentWork);

                //���� Panel�� UpdatePanel �ݰ�, ���� Panel�� DownPanel ����
                updatePanel?.SetActive(false);
                checkDownLoadPanel?.SetActive(true);
                _isCheckUpdate = true;
            }
        }
        else
        {
            Debug.Log("������Ʈ ����" + appUpdateInfoOperation.Error);
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
            LoadData();

        }
        else
        {
            Debug.Log("�α��� ����");
        }
    }

    //Ŭ���忡 Data�����ϱ�
    public void SaveData()
    {
        //Ŭ���� ����ҿ� ��ȣ�ۿ� ������ �������̽�
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        // 1��° ���� �����̸�, 2��° ��Ʈ��ũ ����� ���¿����� Data ����� �ҷ����� ����
        // 3��° ���� �������� ���������� ����� ������ ������, 4��° ���� �ݹ� �Լ�
        savedGameClient.OpenWithAutomaticConflictResolution(_saveFileName, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLastKnownGood, OnSaveDataOpend);
    }

    //����� ���� �����Ϳ� ���� ��û ��� ���¸� �ٷ� �������̽�, ����� ������ ��Ÿ �����͸� �ٷ� �������̽�
    private void OnSaveDataOpend(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Save���� ����");

            var update = new SavedGameMetadataUpdate.Builder().Build();

            //json
            var json = JsonUtility.ToJson(PlayerController.Instance.PlayerData);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            Debug.Log($"���� ������: {bytes}");

            savedGameClient.CommitUpdate(game, update, bytes, OnSaveDataWritten);


        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("Save���� ����");
            Debug.Log($"{status}");
        }

    }

    private void OnSaveDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("���� ����");
        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("���� ����");
        }
    }

    //Ŭ���忡�� Data ��������
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
            Debug.Log("Load ���� ����");

            savedGameClient.ReadBinaryData(data, OnLoadDataRead);
        }
        else
        {
            NetworkCheckManager.Instance.NetWorkErrorPanel.SetActive(true);
            Debug.Log("Load ���� ����");
        }
    }

    private void OnLoadDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        string data = Encoding.UTF8.GetString(loadedData);

        if (data == "")
        {
            Debug.Log("����� �����Ͱ� ����");
        }
        else
        {
            Debug.Log($"Load Read Data: {data}");

            //json
            PlayerController.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(data);
        }
    }

    //Data ����
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
            Debug.Log("������ ���� ����");
            PlayerController.Instance.SetClear();
        }
        else
        {
            Debug.Log("������ ���� ����");
        }
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
