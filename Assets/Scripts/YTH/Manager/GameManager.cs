using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> JewelChanged;

    private int _jewel = 1000;
    public int Jewel
    {
        get => _jewel;
        set { if (_jewel != value) { _jewel = value; JewelChanged?.Invoke(_jewel); } }
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

    [Header("각 스테이지 사운드")]
    [SerializeField] List<AssetReferenceT<AudioClip>> _stageBGM;


    private AudioSource _audioSource;

    public Action SetGameEndHandler;

    //리더보드 이름저장
    private List<string> _leaderboardString = new List<string>();

    private void Awake()
    {
        SetSingleton();

        _audioSource = GetComponent<AudioSource>();
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
        _leaderboardString.Add(GPGSIds.leaderboard_the_first_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_second_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_third_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_fourth_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_last_sun_record);

        WaveManager.Instance.ClearStage += ClearStage;
        PlayStageBGM(0);
        /*StartTimer();*/
    }

    private void OnDestroy()
    {
        WaveManager.Instance.ClearStage -= ClearStage;
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            ClearTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.ShowPanel("ClearPanel");
        }
    }

    public void StartTimer()
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

    public void PlayStageBGM(int stage)
    {
        if (!_playerData.IsSound) return;

        AddressableManager.Instance.LoadSound(_stageBGM[stage], _audioSource, () =>
        {
            _audioSource.loop = true;
            _audioSource.Play();
        });

        Debug.Log($"사운드 실행@@@@@@@");
    }

    public void SetSound()
    {
        _playerData.IsSound = !_playerData.IsSound;
    }

    public void ClearStage()
    {
        if (_playerData != null)
        {
            _playerData.IsClearStage[_playerData.CurrentStage] = true;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => StopTimer())
            .AppendCallback(() => StartCoroutine(WaitForNetworkAndSave()));
    }

    private IEnumerator WaitForNetworkAndSave()
    {
        while (!NetworkCheckManager.Instance.IsConnected)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 연결되었으므로 저장 진행
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
            {
                StartCoroutine(RecordClearTime());
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

        sequence.AppendCallback(() => UIManager.Instance.ShowPanelTemp("GameEndPanel", 3))
                .AppendCallback(() => SetGameEndHandler?.Invoke());
    }

    /// <summary>
    /// 기록이 없거나(0), 더 짧은 시간일 때만 저장
    /// </summary>
    WaitForSeconds waitConnect = new WaitForSeconds(0.5f);
    IEnumerator WaitForNetwork()
    {
        while (NetworkCheckManager.Instance.IsConnected == false)
        {
            yield return waitConnect;
        }
    }

    IEnumerator RecordClearTime()
    {
        int stage = _playerData.CurrentStage;
        float prevTime = _playerData.ClearTimes[stage];

        string leaderboardSt = _leaderboardString[stage]; ;

        //안전하게 네트워크 다시 확인 연결될때까지 기다림
        yield return StartCoroutine(WaitForNetwork());

        if (prevTime == 0f || ClearTime < prevTime)
        {
            _playerData.ClearTimes[stage] = ClearTime;
            GpgsManager.Instance.UpdateTimeLeaderboard(prevTime, leaderboardSt, (success) =>
            {
                if (success == true)
                {
                    Sequence sequence = DOTween.Sequence();


                    sequence.AppendCallback(() => UIManager.Instance.ShowPanel("ClearPanel"))
                                .AppendCallback(() => SetGameEndHandler?.Invoke());
                }
                else
                {
                    //실패시
                    //ex) 다시 시도
                    StartCoroutine(RecordClearTime());
                }
            });
        }
    }

    public bool TutorialCompleted()
    {
        return _playerData.IsTutorial = true;
    }


}