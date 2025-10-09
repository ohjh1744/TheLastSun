using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] AudioClip _stage1;
    [SerializeField] AudioClip _stage2;
    [SerializeField] AudioClip _stage3;
    [SerializeField] AudioClip _stage4;
    [SerializeField] AudioClip _stage5;

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
        PlayStageBGM(_playerData.CurrentStage);
        StartTimer();
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

    public void PlayStageBGM(int stage)
    {
        if (!_playerData.IsSound) return;
        switch (stage)
        {
            case 0:
                _audioSource.clip = _stage1;
                break;
            case 1:
                _audioSource.clip = _stage2;
                break;
            case 2:
                _audioSource.clip = _stage3;
                break;
            case 3:
                _audioSource.clip = _stage4;
                break;
            case 4:
                _audioSource.clip = _stage5;
                break;
            default:
                Debug.LogWarning("Invalid stage number for BGM.");
                return;
        }
        _audioSource.loop = true;
        _audioSource.Play();
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

        string leaderboardSt = _leaderboardString[PlayerController.Instance.PlayerData.CurrentStage]; ;

        //안전하게 네트워크 다시 확인 연결될때까지 기다림
        yield return StartCoroutine(WaitForNetwork());

        if (prevTime == 0f || ClearTime < prevTime)
        {
            _playerData.ClearTimes[stage] = ClearTime;
            GpgsManager.Instance.UpdateTimeLeaderboard(15000, leaderboardSt, (success) =>
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