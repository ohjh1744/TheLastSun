using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class MobControlTower : MonoBehaviour
{
    [Header("몬스터 소환 위치 및 이동 위치")]
    [SerializeField] private Vector3 _spawnPos;
    [SerializeField] private Vector3[] _movPos;

    [Header("한바퀴 도는데 걸리는 이동 시간")]
    //한바퀴 도는데 걸리는 시간 
    [SerializeField] private float[] _mobMoveTimes;

    [Header("몬스터 소환 주기")]
    //웨이브 소환주기
    [SerializeField] private float _spawnDurate;

    [Header("웨이브 별 몬스터 소환 갯수")]
    //웨이브별 소환유닛
    [SerializeField] private int[] _spawnNumForWave;

    private int _curSpawnNumForWave;

    WaitForSeconds _spawnDurateWs;
    Coroutine _spawnRotine;

    ObjectPoolManager _poolManager;
    InGameManager _inGameManager;

    private bool _isWaveChange = false;

    MobController _bossController;
    Canvas _bossCanvas;

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

            if(_inGameManager.WaveNum == _inGameManager.WaveTimes.Length - 1)
            {
                _bossController = mob.GetComponentInChildren<MobController>();
                _bossCanvas = mob.GetComponentInChildren<Canvas>();

                Vector3 originalScale = _bossController.transform.localScale;
                Vector3 canvasOriginalScale = _bossCanvas.transform.localScale;

                //2. 보스는 소환 후 flip하며 움직이도록
                mob.transform.DOPath(_movPos, _mobMoveTimes[_inGameManager.WaveNum], PathType.Linear, PathMode.TopDown2D)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                     .OnWaypointChange((index) =>
                     {
                         Vector3 scale = _bossController.transform.localScale;
                         if (index == 2 && _bossController.IsBoss == true)
                         {
                             scale.x = -Mathf.Abs(originalScale.x); // 반전된 방향
                         }
                         else if (index == 4 && _bossController.IsBoss == true)
                         {
                             scale.x = Mathf.Abs(originalScale.x);  // 원래 방향
                         }
                         _bossController.transform.localScale = scale;
                         _bossCanvas.transform.localScale = canvasOriginalScale;
                     });
            }
            else
            {
                //2. 일반 몹들은 소환 후 움직이도록
                mob.transform.DOPath(_movPos, _mobMoveTimes[_inGameManager.WaveNum], PathType.Linear, PathMode.TopDown2D)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
            }

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
