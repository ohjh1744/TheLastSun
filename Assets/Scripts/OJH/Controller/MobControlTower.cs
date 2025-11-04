using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Init()
    {
        _poolManager = ObjectPoolManager.Instance;
        _inGameManager = InGameManager.Instance;
        _spawnDurateWs = new WaitForSeconds(_spawnDurate);
        _poolManager.MobNumOnChanged += ClearFail;
    }

    private void CheckWaveTime()
    {
        if (_inGameManager.CurrentWaveTime <= 0.1f && _isWaveChange == false)
        {
            _isWaveChange = true;
            _inGameManager.CurrentWaveTime = 0f;
            _inGameManager.WaveNum++;
            Debug.Log($"WaveNum증가{_inGameManager.WaveNum}");
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

            //2. 소환 후 움직이도록
            mob.transform.DOPath(_movPos, _mobMoveTimes[_inGameManager.WaveNum], PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            yield return _spawnDurateWs;
        }
    }

    private void ClearFail()
    {
        //100마리 넘엇을 시 
        if(_poolManager.MobNum >= _inGameManager.MobNumForDefeat)
        {
            _inGameManager.GameState = EGameState.Defeat;
        }

        //중간보스, 보스 안죽엇을시
        if(_inGameManager.WaveNum == 26 || _inGameManager.WaveNum == 50)
        {
            if(_poolManager.MobPools[_inGameManager.WaveNum][0].activeSelf == true)
            {
                _inGameManager.GameState = EGameState.Defeat;
            }
        }
    }

    
}
