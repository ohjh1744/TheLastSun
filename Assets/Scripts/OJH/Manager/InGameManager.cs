using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum EGameState {Ready, Play, Defeat, Win }
public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private int _jemNum;
    public int JemNum { get { return _jemNum; } set { _jemNum = value; JemNumOnChanged?.Invoke(); } }
    public event UnityAction JemNumOnChanged;

    private float _playTime; public float PlayTime { get { return _playTime; } set { _playTime = value; } }

    private float[] _waveTimes; public float[] WaveTimes { get { return _waveTimes; } private set { } }
    private float _currentWaveTime; public float CurrentWaveTime { get { return _currentWaveTime; } private set { } }

    private int _waveNum; public int WaveNum { get { return _waveNum; } }

    [SerializeField] private int _mobNumForDefeat; public int MobNumForDefeat { get { return _mobNumForDefeat; } private set { } }

    [SerializeField] private int _mobNumForDefeatWarning; public int MobNumForDefeatWarning { get { return _mobNumForDefeatWarning; } private set { } }

    [SerializeField] private float _checkLoadFinishTime;
    WaitForSeconds _checkisLoadFinishWs;

    [SerializeField] private GameObject _LoadingPanel;
    [SerializeField] private GameObject _toturiolPanel;
    [SerializeField] private GameObject _clearPanel;

    [SerializeField] private EGameState _gameState; public EGameState GameState { get { return _gameState; }set { _gameState = value; } }

    //리더보드 이름저장
    private List<string> _leaderboardString = new List<string>();

    //게임 시작, 종료 관리
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
        StartGame();
    }

    private void Init()
    {
        _checkisLoadFinishWs = new WaitForSeconds(_checkLoadFinishTime);
        _leaderboardString.Add(GPGSIds.leaderboard_the_first_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_second_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_third_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_fourth_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_last_sun_record);
    }

    private void StartGame()
    {
        StartCoroutine(CheckHasTutorial());
    }

    private void Update()
    {
        PlayGame();
    }


    IEnumerator CheckHasTutorial()
    {
        //로딩끝났는지확인
        while (true)
        {
            if(_LoadingPanel.activeSelf == false)
            {
                break;
            }
            yield return _checkisLoadFinishWs;
        }

        //튜토리얼 경험이 없다면 튜토리얼 시작
        if(PlayerController.Instance.PlayerData.IsTutorial == false)
        {
            _toturiolPanel.SetActive(true);
        }
        //아니라면 게임 시작상태로 변경 
        else
        {
            _gameState = EGameState.Play;
        }
    }

    private void PlayGame()
    {
        if(_gameState == EGameState.Play)
        {
            CheckWaveTime();
            CheckPlayTime();
            CheckEndGame();
        }
    }

    private void CheckWaveTime()
    {
        if (_currentWaveTime <= 0f)
        {
            _currentWaveTime = _waveTimes[_waveNum];
            _waveNum++;
        }
        _currentWaveTime -= Time.deltaTime;
    }
    private void CheckPlayTime()
    {
        _playTime += Time.deltaTime;
    }
    private void CheckEndGame()
    {
        // 진경우 ,이긴경우
        // 클리어 패널 열어주기
        if (_gameState == EGameState.Win)
        {
            StartCoroutine(UpdateLeaderboard());
        }
        else if(_gameState == EGameState.Defeat)
        {
            _clearPanel.SetActive(true);
        }

    }

    IEnumerator UpdateLeaderboard()
    {
        PlayerData playerData = PlayerController.Instance.PlayerData;
        playerData.ClearTimes[playerData.CurrentStage] = Mathf.Max(playerData.ClearTimes[playerData.CurrentStage], _playTime);

        // 안전하게 네트워크 체킹 한번더 
        while(NetworkCheckManager.Instance.IsConnected != true)
        {
            yield return null;
        }

        GpgsManager.Instance.UpdateTimeLeaderboard(playerData.ClearTimes[playerData.CurrentStage], _leaderboardString[playerData.CurrentStage], (success) =>
        {
            if (success)
            {
                _clearPanel.SetActive(true);
            }
        });
    }

}
