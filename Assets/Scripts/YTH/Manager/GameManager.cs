using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> JewelChanged;

    private int _jewel = 1000;
    public int Jewel
    {
        get => _jewel;
        set
        {
            if (_jewel != value)
            {
                _jewel = value;
                JewelChanged?.Invoke(_jewel);
            }
        }
    }

    private PlayerData _playerData => PlayerController.Instance.PlayerData;

    public float ClearTime;

    private bool _isTimerRunning = false;

    private bool _isPause;
    public bool IsPause
    {
        get => _isPause;
        set
        {
            _isPause = value;
            Time.timeScale = _isPause ? 0 : CurrentGameSpeed;
        }
    }

    public int CurrentGameSpeed = 1;

    private void Awake()
    {
        SetSingleton();
    }

    #region 싱글톤 세팅
    public void SetSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            ClearTime += Time.deltaTime;
        }
    }

    private void StartTimer()
    {
        ClearTime = 0f;
        _isTimerRunning = true;
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    public void SetGameSpeed()
    {
        CurrentGameSpeed = CurrentGameSpeed >= 3 ? 1 : CurrentGameSpeed + 1;
        Time.timeScale = CurrentGameSpeed;
    }

    public void PauseGame()
    {
        IsPause = !_isPause;
        Debug.Log("IsPause: " + IsPause);
    }

    public void ClearStage()
    {
        StopTimer();

        _playerData.IsClearStage[_playerData.CurrentStage] = true;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => StopTimer())
            .AppendCallback(() => StartCoroutine(WaitForNetworkAndSave()))
            .AppendCallback(() => UIManager.Instance.ShowPanelTemp("ClearPanel", 3))
            .AppendInterval(3)
            //TODO: 3초 후 메인 씬으로 이동을 버튼 눌러 메인으로 이동하는 것으로 변경
            .AppendCallback(() => SceneManager.LoadScene(1));


    }

    private IEnumerator WaitForNetworkAndSave()
    {
        // 네트워크 연결될 때까지 0.5초 간격으로 대기
        while (!NetworkCheckManager.Instance.IsConnected)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 연결되었으므로 저장 진행
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
            {
                RecordClearTime();
            }
            else
            {
                Debug.Log("네트워크 연결 실패...");
                // TODO: 재시도 로직 등
            }
        });
    }

    public void FailStage()
    {
        Debug.Log("Stage Failed");

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => UIManager.Instance.ShowPanelTemp("GameOverPanel", 3))
            .AppendInterval(3)
            .AppendCallback(() => SceneManager.LoadScene(1));
    }

    /// <summary>
    /// 기록이 없거나(0), 더 짧은 시간일 때만 저장
    /// </summary>
    private void RecordClearTime()
    {
        int stage = _playerData.CurrentStage;
        float prevTime = _playerData.ClearTimes[stage];


        if (prevTime == 0f || ClearTime < prevTime)
        {
            _playerData.ClearTimes[stage] = ClearTime;
        }
    }

    public bool TutorialCompleted()
    {
        return _playerData.IsTutorial = true;
    }
}
