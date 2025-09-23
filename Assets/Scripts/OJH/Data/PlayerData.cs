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
    //_currentStage 변경은 인게임과는 상관 x.
    // 0~4 존재.
    [SerializeField] private int _currentStage;
    public int CurrentStage { get { return _currentStage;} set { _currentStage = value; OnCurrentStageChanged?.Invoke(); } }

    //각 스테이지 클리어 여부
    //스테이지 클리어했다면 인게임에서 해당 변수 true로 변경하고 저장하기 GPGSManager(SaveData)
    [SerializeField] private bool[] _isClearStage;
    public bool[] IsClearStage {  get { return _isClearStage; } set { _isClearStage = value; } }

    [SerializeField] private bool _isSound;
    public bool IsSound { get { return _isSound; } set { _isSound = value; } }

    //아래 배열처럼 new int[]식으로 크기를 정해주어야, 나중에 배열길이 Update되어도 마이그레이션가능
    // 또한 이전 Data에는 포함되지 않은 배열이었을 경우, 아래와 같이 지정해주어야 그대로 유지. 안하면 배열크기 초기화 됨
    //[SerializeField] private int[] _tests = new int[6];
    //public int[] Tests { get { return _tests; } set { _tests = value; } }
    //이벤트
    public event UnityAction OnCurrentStageChanged;
}
