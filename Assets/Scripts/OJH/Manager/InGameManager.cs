using GooglePlayGames.BasicApi.SavedGame;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum EGameState {Ready,Pause, Play, Defeat, Win }
public enum EUpgrade { Warrior, Archer, Bomer, None}
public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance { get { return _instance; } set { _instance = value; } }

    [Header("소유하는 보석 개수")]
    [SerializeField] private int _jemNum;
    public int JemNum { get { return _jemNum; } set { _jemNum = value; JemNumOnChanged?.Invoke(); } }
    public event UnityAction JemNumOnChanged;

    [Header("일반 소환 및 특수 소환 시 필요한 개수")]
    [SerializeField] private int _normalSpawnForJemNum; public int NormalSpawnForJemNum { get { return _normalSpawnForJemNum; } private set {} }
    [SerializeField] private int _specialSpawnForJemNum; public int SpecialSpawnForJemNum { get { return _specialSpawnForJemNum; } private set {} }

    [Header("일반 소환 및 특수 소환 확률")]
    [SerializeField] private float[] _normalSpawnRates; public float[] NormalSpawnRates { get { return _normalSpawnRates; } private set { } }
    [SerializeField] private float[] _specialSpawnRates; public float[] SpecialSpawnRates { get { return _specialSpawnRates; } private set { } }

    [Header("등급 별 보석 판매 개수")]
    [SerializeField] private int[] _jemNumsForSell; public int[] JemNumsForSell { get { return _jemNumsForSell; } private set { } }

    [Header("배속 관련")]
    [SerializeField] private float[] _speedUpRate; public float[] SpeedUpRate { get { return _speedUpRate; } private set { } }
    private int _speedUpIndex;public int SpeedUpIndex { get { return _speedUpIndex; } set { _speedUpIndex = value; } }

    private float _playTime; public float PlayTime { get { return _playTime; } set { _playTime = value; } }

    [Header("웨이브별 시간")]
    [SerializeField]private float[] _waveTimes; public float[] WaveTimes { get { return _waveTimes; } private set { } }
    private float _currentWaveTime; public float CurrentWaveTime { get { return _currentWaveTime; }  set { _currentWaveTime = value; CurrentWaveTimeOnChanged?.Invoke(); } }
    public event UnityAction CurrentWaveTimeOnChanged;

    [Header("현재 웨이브")]
    [SerializeField] private int _waveNum; public int WaveNum { get { return _waveNum; } set { _waveNum = value; CurrentWaveNumOnChanged?.Invoke(); } }
    public event UnityAction CurrentWaveNumOnChanged;

    [Header("실패 관련 최대 몬스터 수 ")]
    [SerializeField] private int _mobNumForDefeat; public int MobNumForDefeat { get { return _mobNumForDefeat; } private set { } }

    [Header("몬스터 수에 따른 경고")]
    [SerializeField] private int[] _mobNumForDefeatWarning; public int[] MobNumForDefeatWarning { get { return _mobNumForDefeatWarning; } private set { } }

    [Header("게임 상태")]
    [SerializeField] private EGameState _gameState; public EGameState GameState { get { return _gameState; }set { _gameState = value; } }

    [Header("강화 관련")]
    [SerializeField] private int[] _jemNumsForUpgrade; public int[] JemNumsForUpgrade { get { return _jemNumsForUpgrade; } set { _jemNumsForUpgrade = value; } }
    [SerializeField] private int[] _jemNumPlusForUpgrade; public int[] JemNumPlusForUpgrade { get { return _jemNumPlusForUpgrade; } set { _jemNumPlusForUpgrade = value; } }
    [SerializeField] private int[] _upgradeLevels; public int[] UpgradeLevels { get { return _upgradeLevels; } set { _upgradeLevels = value; } }
    public UnityAction<EUpgrade>[] OnChangedUpgradeLevels = new UnityAction<EUpgrade>[3];
    [SerializeField] private int[] _upgradeStats; public int[] UpgradeStats { get { return _upgradeStats; } set { _upgradeStats = value; } }


    [Header("소환할 수 있는 최대 영웅 수")]
    [SerializeField] private int _maxHeroNum; public int MaxHeroNum { get { return _maxHeroNum; } private set { }}

    [SerializeField] private GameObject _LoadingPanel;
    [SerializeField] private GameObject _toturiolPanel;
    [SerializeField] private GameObject _clearPanel;

    //리더보드 이름저장
    private List<string> _leaderboardString = new List<string>();

    WaitForSecondsRealtime _clearWs = new WaitForSecondsRealtime(0.1f);
    Coroutine _endGameRoutine;


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

    private void Update()
    {
        SetGameSpeed();
        PlayGame();
        CheckEndGame();
    }


    private void Init()
    {
        _leaderboardString.Add(GPGSIds.leaderboard_the_first_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_second_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_third_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_fourth_sun_record);
        _leaderboardString.Add(GPGSIds.leaderboard_the_last_sun_record);

        _currentWaveTime = _waveTimes[0];
    }

    private void StartGame()
    {
        StartCoroutine(CheckHasTutorial());
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
            yield return null;
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

    // 정지와 배속에 따른 게임 스피드 조정
    private void SetGameSpeed()
    {
        //이게 있어야 로딩창 및 인게임중 네트워크 문제 생겨야 멈춤
        if (NetworkCheckManager.Instance.IsConnected == false || _gameState == EGameState.Pause || _gameState == EGameState.Defeat || _gameState == EGameState.Win)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = SpeedUpRate[SpeedUpIndex];
        }
    }

    private void PlayGame()
    {
        if(_gameState == EGameState.Play)
        {
            CheckPlayTime();
        }
    }
    private void CheckPlayTime()
    {
        _playTime += Time.deltaTime;
    }

    public void TempClearGame()
    {
        _gameState = EGameState.Win;
    }
    private void CheckEndGame()
    {
        // 진경우 ,이긴경우
        // 클리어 패널 열어주기
        if (_gameState == EGameState.Win)
        {
            if(_endGameRoutine == null)
            {
                _endGameRoutine = StartCoroutine(SaveDataAndUpdateLeaderboard());
            }
        }
        else if(_gameState == EGameState.Defeat)
        {
            _clearPanel.SetActive(true);
        }

    }

    IEnumerator SaveDataAndUpdateLeaderboard()
    {
        PlayerData playerData = PlayerController.Instance.PlayerData;
        playerData.IsClearStage[playerData.CurrentStage] = true;

        //아직 클리어하지않은 경우 즉, 0으로 되어있다면.
        if (playerData.ClearTimes[playerData.CurrentStage] < 1f){
            playerData.ClearTimes[playerData.CurrentStage] = _playTime;
        }
        //클리어한적이 있다면 가장 작은값을 기록
        else
        {
            playerData.ClearTimes[playerData.CurrentStage] = Mathf.Min(playerData.ClearTimes[playerData.CurrentStage], _playTime);
        }

        bool _isDataSave = false;
        bool _isUpdateTime = false;

        // 안전하게 네트워크 체킹 한번더 
        while(NetworkCheckManager.Instance.IsConnected == false)
        {
            yield return _clearWs;
        }

        GpgsManager.Instance.SaveData((success) =>
        {
            if (success == SavedGameRequestStatus.Success)
            {
                _isDataSave = true;
            }
        });

        GpgsManager.Instance.UpdateTimeLeaderboard(playerData.ClearTimes[playerData.CurrentStage], _leaderboardString[playerData.CurrentStage], (success) =>
        {
            if (success == true)
            {
                _isUpdateTime = true;
            }
        });

        // 네트워크 연결 & 데이터 세이브 & 업데이트 완료 시까지 기다리기
        while (NetworkCheckManager.Instance.IsConnected == false || _isDataSave == false || _isUpdateTime == false)
        {
            yield return _clearWs;
        }

        _clearPanel.SetActive(true);
    }

    public void SetUpgradeLevel(EUpgrade upgradeType, int newLevel)
    {
        _upgradeLevels[(int)upgradeType] = newLevel;
        OnChangedUpgradeLevels[(int)upgradeType]?.Invoke(upgradeType);
    }

}
