using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Don't Set")]
    public int _spawnedMonsterCount = 0;
    public int _spawnBoss;
    public int _aliveMonsterCount = 0;
    public int _deadMonsterCount = 0;

    [Header("Set")]
    [SerializeField] int _totalWave = 100;
    [SerializeField] int _monstersPerWave = 12;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] List<GameObject> _bossPrefabs;

    private ObservableProperty<int> AliveMonsterCount => new ObservableProperty<int>(_aliveMonsterCount);
    private ObservableProperty<int> _currentWave = new ObservableProperty<int>(1);
    private ObservableProperty<int> DeadMonsterCount => new ObservableProperty<int>(_deadMonsterCount);

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(2f);
    private WaitForSeconds _waveDelay = new(3f);

    private void Awake()
    {
        _objectPool = GetComponent<ObjectPool>();
    }

    #region 옵저버 패턴 구독
    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }
 
    private void Subscribe()
    {
        _currentWave.Subscribe(OnWaveChanged);
        AliveMonsterCount.Subscribe(OnStageFail);
        DeadMonsterCount.Subscribe(OnClearStage);
    }

    private void UnSubscribe()
    {
        _currentWave.Unsubscribe(OnWaveChanged);
        AliveMonsterCount.Unsubscribe(OnStageFail);
        DeadMonsterCount.Unsubscribe(OnClearStage);
    }
    #endregion

    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (_currentWave.Value <= _totalWave)
        {
            for (int i = 0; i < _monstersPerWave; i++)
            {
                SpawnMonster(isBoss: false);
                yield return _spawnDelay;
            }

            _currentWave.Value++;
            yield return _waveDelay; // 웨이브 간 대기
        }
    }

    // 웨이브 변경 이벤트 핸들러
    private void OnWaveChanged(int wave)
    {
        if (wave == 10 || wave == 30 || wave == 50 || wave == 70 || wave == 100)
        {
            SpawnMonster(isBoss: true);
        }
    }

    private void OnStageFail(int count)
    {
        if (count > 50)
        {
            GameManager.Instance.FailStage();
        }
    }

    private void OnClearStage(int count)
    {
        if (count >= _totalWave * _monstersPerWave)
        {
            GameManager.Instance.ClearStage();
        }
    }

    private void SpawnMonster(bool isBoss)
    {
        if (isBoss)
        {
            // 보스 몬스터 소환
            GameObject BossInstance = Instantiate(_bossPrefabs[_spawnBoss], _spawnPoint.position, Quaternion.identity);
            _spawnBoss++;
            Debug.Log("보스 소환");
        }
        else
        {
            // 일반 몬스터 풀에서 소환
            PooledObject pooledObject = _objectPool.GetPool(_spawnPoint.position, Quaternion.identity);
        }
        _spawnedMonsterCount++;
        _aliveMonsterCount++;
    }
}
