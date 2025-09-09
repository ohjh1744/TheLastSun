using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class PlayerData
{
    //각 스테이지별 플레이어가 클리어한 타임 
    //클리어시,인게임에서 해당변수 Update하고, 이 기록을 GPGSManager 함수활용해 저장하고(SaveData), 또 업데이트하기(UpdateTimeLeaderboard)
    [SerializeField] private float[] _clearTimes;
    public float[] ClearTimes { get { return _clearTimes; } set { _clearTimes = value; } }

    //플레이어가 플레이 하기 위해 현재 고른 스테이지
    //이 변수를 활용해 인게임에서 그에 맞는 스테이지 불러오기
    //_currentStage 설정은 아웃게임에서 하기에 인게임에서는 신경x.
    [SerializeField] private int _currentStage;

    public int CurrentStage { get { return _currentStage;} set { _currentStage = value; OnCurrentStageChanged?.Invoke(); } }

    //각 스테이지 클리어 여부
    //스테이지 클리어했다면 인게임에서 해당 변수 Update하고 저장하기 GPGSManager(SaveData)
    [SerializeField] private bool[] _isClearStage;

    public bool[] IsClearStage {  get { return _isClearStage; } set { _isClearStage = value; } }


    //이벤트
    public event UnityAction OnCurrentStageChanged;
}
