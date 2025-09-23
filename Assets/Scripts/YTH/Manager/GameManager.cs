using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerData _playerData;

    private float _clearTime;

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

    #region ½Ì±ÛÅæ ¼¼ÆÃ
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
        if (_isTimerRunning)
        {
            _clearTime += Time.deltaTime;
        }
    }

    private void StartTimer()
    {
        _clearTime = 0f;
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
    }

    public void ClearStage()
    {
        _playerData.IsClearStage[_playerData.CurrentStage] = true;
        StopTimer();
        RecordClearTime();
    }

    private void RecordClearTime()
    {
        _playerData.ClearTimes[_playerData.CurrentStage] = _clearTime;
    }
}
