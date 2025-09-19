using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerData _playerData;

    private float _clearTime;

    private void Awake()
    {
        SetSingleton();
    }

    #region ΩÃ±€≈Ê ºº∆√
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
        _clearTime += Time.deltaTime;
    }

    public void SetGameSpeed(float value)
    {
        Time.timeScale = value;
    }

    public void ClearStage()
    {
        _playerData.IsClearStage[_playerData.CurrentStage] = true;
    }

    public void RecordClearTime()
    {
        _playerData.ClearTimes[_playerData.CurrentStage] = _clearTime;
    }
}
