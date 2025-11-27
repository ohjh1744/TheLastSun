using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private PlayerData _playerData;
    public PlayerData PlayerData { get { return _playerData; } set { _playerData = value; } }

    private void Awake()
    {
        Debug.Log("GPGSManager Awake");
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Data 클리어 Test중에 아마 사용할 함수
    public void SetClear()
    {
        for (int i = 0; i < _playerData.ClearTimes.Length; i++)
        {
            _playerData.ClearTimes[i] = 0;
            _playerData.IsClearStage[i] = false;
        }

        _playerData.CurrentStage = 0;

        _playerData.IsSound = true;

        _playerData.IsTutorial = false;

    }

}
