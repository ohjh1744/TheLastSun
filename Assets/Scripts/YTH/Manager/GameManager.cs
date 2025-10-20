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

        if (_playerData.IsSound)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }

    public void ClearStage()
    {
        PauseGame();

        if (_playerData != null)
        {
            // isclearStage 업데이트
            _playerData.IsClearStage[_playerData.CurrentStage] = true;
            //기존에 클리어 타임이 없거나 기존의 클리어타임보다 더 빨리 클리어한 경우 업데이트
            if(_playerData.ClearTimes[_playerData.CurrentStage] == 0 || ClearTime < _playerData.ClearTimes[_playerData.CurrentStage])
            {
                _playerData.ClearTimes[_playerData.CurrentStage] = ClearTime;
            }
        }

        Sequence sequence = DOTween.Sequence();

        sequence
            .SetUpdate(true)
            .AppendCallback(() => StopTimer())
            .AppendCallback(() => StartCoroutine(WaitForNetworkAndSave()));
    }

    private IEnumerator WaitForNetworkAndSave()
    {
        Debug.Log("웨잇포네트워크앤세이브 시작");

        yield return StartCoroutine(WaitForNetwork());

        Debug.Log("웨잇포네트워크앤세이브 네트워크체크완료");

        // 연결되었으므로 저장 진행
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
            {
                Debug.Log("웨잇포네트워크 세이브 성공");
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

        sequence.AppendCallback(() => StopTimer())
            .AppendCallback(() => UIManager.Instance.ShowPanel("GameEndPanel"))
                .AppendCallback(() => SetGameEndHandler?.Invoke());
    }

    /// <summary>
    /// 기록이 없거나(0), 더 짧은 시간일 때만 저장
    /// </summary>
    WaitForSecondsRealtime waitConnect = new WaitForSecondsRealtime(0.5f);
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
        string leaderboardSt = _leaderboardString[stage];

        Debug.Log("리코드클리어타임시작");

        //안전하게 네트워크 다시 확인 연결될때까지 기다림
        yield return StartCoroutine(WaitForNetwork());

        Debug.Log("리코드클리어타임 네트워크체크완료");

        GpgsManager.Instance.UpdateTimeLeaderboard(_playerData.ClearTimes[stage], leaderboardSt, (success) =>
        {
            if (success == true)
            {
                Debug.Log("리코드클리어타임 완료");
                Sequence sequence = DOTween.Sequence();

                sequence.AppendCallback(() => UIManager.Instance.ShowPanel("ClearPanel"))
                            .AppendCallback(() => SetGameEndHandler?.Invoke());
            }
            else
            {
                //다시 시도
                StartCoroutine(RecordClearTime());
            }
        });

    }

    public bool TutorialCompleted()
    {
        return _playerData.IsTutorial = true;
    }
}