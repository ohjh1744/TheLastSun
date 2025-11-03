using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobControlTower : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPos;
    [SerializeField] private Vector3[] _movPos;

    //한바퀴 도는데 걸리는 시간 
    [SerializeField] private float[] _mobMoveTime;

    //웨이브 소환주기
    [SerializeField] private float _spawnDurate;
    WaitForSeconds _spawnDurateWs;
    Coroutine _spawnRotine;

    ObjectPoolManager _poolManager;
    InGameManager _inGameManager;
    private void Start()
    {
        _poolManager = ObjectPoolManager.Instance;
        _inGameManager = InGameManager.Instance;
        _spawnDurateWs = new WaitForSeconds(_spawnDurate);
    }

    private void Update()
    {
        //게임상태가 플레이가 아니거나, 네트워크문제있으면 return.
        if (_inGameManager.GameState != EGameState.Play || NetworkCheckManager.Instance.IsConnected == false)
            return;


        CheckWaveTime();
        StartSpawnMobAndMove();
    }

    private void CheckWaveTime()
    {
        if (_inGameManager.CurrentWaveTime <= 0.1f)
        {
            _inGameManager.CurrentWaveTime = 0f;
            _inGameManager.WaveNum++;
            _inGameManager.CurrentWaveTime = _inGameManager.WaveTimes[_inGameManager.WaveNum];
        }

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
            //1. 소환
            GameObject mob = _poolManager.GetObject(_poolManager.MobPools, _poolManager.Mobs, _inGameManager.WaveNum);
            mob.transform.position = _spawnPos;

            //2. 소환 후 움직이도록
            mob.transform.DOPath(_movPos, _mobMoveTime[_inGameManager.WaveNum], PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            yield return _spawnDurateWs;
        }
    }

    
}
