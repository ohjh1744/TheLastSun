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
                        Debug.Log("�ٿ�� �Ϸ�");
                    }
                    yield return null;
                }

                //���� ��ġ
                var result = _appUpdateManager.CompleteUpdate();

                //�Ϸ�Ǿ����� ������ Ȯ��
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            //������Ʈ�� ���� ���¶��
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("������Ʈ ����!");

                yield return _delayToFinishWs;
                //�α����ϱ�
                Login();
                //���� �г� �����ֱ�
                nextPanel.SetActive(true);
                nextText.text = "Check Resource";
            }
        }
        else
        {
            Debug.Log("������Ʈ ����" + appUpdateInfoOperation.Error);
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

            Debug.Log($"�α��� ����{displayName}{userID}");

        }
        else
        {
            Debug.Log("�α��� ����");
        }
    }
}
