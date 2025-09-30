using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerData _playerData => PlayerController.Instance.PlayerData;

    public int Jewel;

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
        _playerData.IsClearStage[_playerData.CurrentStage] = true;
        StopTimer();
        RecordClearTime();
    }

    public void FailStage()
    {

    }

    private void RecordClearTime()
    {
        _playerData.ClearTimes[_playerData.CurrentStage] = ClearTime;
    }

    public bool TutorialCompleted()
    {
        return _playerData.IsTutorial = true;
    }
}
