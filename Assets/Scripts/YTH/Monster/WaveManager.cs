using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
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
    
    //수정할것
    //public ObservableProperty<int> SpawnedMonsterCount = new ObservableProperty<int>(_spawnedMonsterCount);
    private ObservableProperty<int> AliveMonsterCount => new ObservableProperty<int>(_aliveMonsterCount);
    public ObservableProperty<int> CurWave = new ObservableProperty<int>(1);
    private ObservableProperty<int> DeadMonsterCount => new ObservableProperty<int>(_deadMonsterCount);

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(1f);
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
        CurWave.Subscribe(OnWaveChanged);
        AliveMonsterCount.Subscribe(OnStageFail);
        AliveMonsterCount.Subscribe(OnWarning);
        DeadMonsterCount.Subscribe(OnClearStage);
    }

    private void UnSubscribe()
    {
        CurWave.Unsubscribe(OnWaveChanged);
        AliveMonsterCount.Unsubscribe(OnStageFail);
        DeadMonsterCount.Unsubscribe(OnClearStage);
        AliveMonsterCount.Unsubscribe(OnWarning);
    }
    #endregion

    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (CurWave.Value <= _totalWave)
        {
            for (int i = 0; i < _monstersPerWave; i++)
            {
                int moveSpeed = _monstersPerWave - i; // 12, 11, ..., 1
                SpawnMonster(isBoss: false, moveSpeed);
                yield return _spawnDelay;
            }

            CurWave.Value++;
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

    private void OnWarning(int count)
    {
        if (count == 40)
        {
            //팝업 온
        }
    }

    private void OnClearStage(int count)
    {
        if (count >= _totalWave * _monstersPerWave)
        {
            GameManager.Instance.ClearStage();
        }
        
    }

    private void SpawnMonster(bool isBoss, int moveSpeed = 1)
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

            // MonsterModel의 MoveSpeed를 소환 순서에 따라 할당
            MonsterModel model = pooledObject.GetComponent<MonsterModel>();
            if (model != null)
            {
                model.MoveSpeed = moveSpeed;
            }
        }
        _spawnedMonsterCount++;
        _aliveMonsterCount++;
    }
}
