using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class MobControlTower : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPos;
    [SerializeField] private Vector3[] _movPos;

    //한바퀴 도는데 걸리는 시간 
    [SerializeField] private float[] _mobMoveTimes;

    //웨이브 소환주기
    [SerializeField] private float _spawnDurate;

    //웨이브별 소환유닛
    [SerializeField] private int[] _spawnNumForWave;

    private int _curSpawnNumForWave;

    WaitForSeconds _spawnDurateWs;
    Coroutine _spawnRotine;

    ObjectPoolManager _poolManager;
    InGameManager _inGameManager;

    private bool _isWaveChange = false;
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        //게임상태가 플레이가 아니거나, 네트워크문제있으면 return.
        if (_inGameManager.GameState != EGameState.Play || NetworkCheckManager.Instance.IsConnected == false)
            return;

        CheckWaveTime();
        StartSpawnMobAndMove();
        CheckClearOrDefeat();
    }

    private void Init()
    {
        _poolManager = ObjectPoolManager.Instance;
        _inGameManager = InGameManager.Instance;
        _spawnDurateWs = new WaitForSeconds(_spawnDurate);
    }

    private void CheckWaveTime()
    {
        if (_inGameManager.CurrentWaveTime <= 0.1f && _isWaveChange == false)
        {
            _isWaveChange = true;
            _inGameManager.CurrentWaveTime = 0f;
            _inGameManager.WaveNum++;
            _inGameManager.CurrentWaveTime = _inGameManager.WaveTimes[_inGameManager.WaveNum];
            _curSpawnNumForWave = 0;
        }
        _isWaveChange = false;
        _inGameManager.CurrentWaveTime -= Time.deltaTime;
    }

    private void StartSpawnMobAndMove()
    {
        if(_spawnRotine == null)
        {
            _spawnRotine = StartCoroutine(SpawnMobsAndMove());
        }
    }

 
    IEnumerator SpawnMobsAndMove()
    {
        while (true)
        {
            if (_curSpawnNumForWave == _spawnNumForWave[_inGameManager.WaveNum])
            {
                yield return null;
                continue;
            }
            //1. 소환
            _curSpawnNumForWave++;
            _poolManager.MobNum++;
            GameObject mob = _poolManager.GetObject(_poolManager.MobPools, _poolManager.Mobs, _inGameManager.WaveNum);
            mob.transform.position = _spawnPos;

            mob.transform.DOKill();

            Vector3 originalScale = mob.transform.localScale;

            //2. 소환 후 움직이도록
            mob.transform.DOPath(_movPos, _mobMoveTimes[_inGameManager.WaveNum], PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                 .OnWaypointChange((index) =>
                 {
                     Vector3 scale = mob.transform.localScale;

                     //보스만 flip
                     if (index == 2 && _inGameManager.WaveNum == 50)
                     {
                         scale.x = -Mathf.Abs(originalScale.x); // 반전된 방향
                     }
                     else if(index == 4 && _inGameManager.WaveNum == 50)
                     {
                         scale.x = Mathf.Abs(originalScale.x);  // 원래 방향
                     }
                     mob.transform.localScale = scale;
                 });

            yield return _spawnDurateWs;
        }
    }

    private void CheckClearOrDefeat()
    {
        //100마리 넘엇을 시 실패
        if(_poolManager.MobNum >= _inGameManager.MobNumForDefeat)
        {
            _inGameManager.GameState = EGameState.Defeat;
        }

        //중간보스, 보스 안죽엇을시 실패
        if((_inGameManager.WaveNum == 24 || _inGameManager.WaveNum == 49) && _inGameManager.CurrentWaveTime <= 0.2f)
        {
            if(_poolManager.MobPools[_inGameManager.WaveNum].Count != 0 && _poolManager.MobPools[_inGameManager.WaveNum][0].activeSelf == true)
            {
                _inGameManager.GameState = EGameState.Defeat;
            }
        }

        //보스 죽었을시 즉시 클리어
        if (_inGameManager.WaveNum == 49)
        {
            if (_poolManager.MobPools[_inGameManager.WaveNum].Count != 0 && _poolManager.MobPools[_inGameManager.WaveNum][0].activeSelf == false)
            {
                _inGameManager.GameState = EGameState.Win;
            }
        }
    }

    
}
