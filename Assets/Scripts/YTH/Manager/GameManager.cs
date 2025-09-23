using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerData _playerData;

    private float _clearTime;
    private bool _isTimerRunning = false;

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

    private void Update()
    {
        // 타이머가 동작 중일 때만 시간 누적
        if (_isTimerRunning)
        {
            _clearTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 게임 시작 시 타이머를 초기화하고 시작
    /// </summary>
    public void StartTimer()
    {
        _clearTime = 0f;
        _isTimerRunning = true;
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    public void SetGameSpeed(float value)
    {
        Time.timeScale = value;
    }

    public void ClearStage()
    {
        _playerData.IsClearStage[_playerData.CurrentStage] = true;
        StopTimer(); // 스테이지 클리어 시 타이머 멈춤
        RecordClearTime(); // 클리어 시간 기록
    }

    public void RecordClearTime()
    {
        _playerData.ClearTimes[_playerData.CurrentStage] = _clearTime;
    }
}
