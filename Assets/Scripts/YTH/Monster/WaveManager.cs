using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private InGameUI _inGameUi;

    [Header("Don't Set")]
    public int _toSpawnBossindex;

    [SerializeField] int _spawnedMonsterCount = 0;
    public int SpawnedMonsterCount { get => _spawnedMonsterCount; set { _spawnedMonsterCount = value; SpawnedMonsterCountChanged?.Invoke(_spawnedMonsterCount); } }

    [SerializeField] int _aliveMonsterCount = 0;
    public int AliveMonsterCount { get => _aliveMonsterCount; set { _aliveMonsterCount = value; AliveMonsterCountChanged?.Invoke(_aliveMonsterCount); } }

    [SerializeField] int _deadMonsterCount = 0;
    public int DeadMonsterCount
    {
        get => _deadMonsterCount;
        set { _deadMonsterCount = value; if (_deadMonsterCount >= _clearCondition) { OnClearStage(_deadMonsterCount); } }
    }

    private int _curWave = 1;
    public int CurWave { get => _curWave; private set { _curWave = value; CurWaveChanged?.Invoke(_curWave); } }

    [Header("Set")]
    [SerializeField] int _totalWave = 100;
    [SerializeField] int _monstersPerWave = 12;
    private int _clearCondition => _totalWave * _monstersPerWave;
    public int ClearCondition => _clearCondition;

    [SerializeField] Transform _spawnPoint;
    [HideInInspector] public Transform SpawnPoint => _spawnPoint;
    [SerializeField] List<GameObject> _bossPrefabs;

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(1f);
    private WaitForSeconds _waveDelay = new(6f);

    public event Action<int> SpawnedMonsterCountChanged;
    public event Action<int> AliveMonsterCountChanged;
    public event Action<int> CurWaveChanged;
    public event Action ClearStage;

    private void Awake()
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

        _inGameUi = FindObjectOfType<InGameUI>();
        _objectPool = GetComponent<ObjectPool>();
    }

    private void OnEnable()
    {
        CurWaveChanged += OnWaveChanged;
        AliveMonsterCountChanged += OnStageFail;
        AliveMonsterCountChanged += OnWarning;
        AliveMonsterCountChanged += _inGameUi.OnAliveMonsterCountChanged;
    }

    private void OnDisable()
    {
        CurWaveChanged -= OnWaveChanged;
        AliveMonsterCountChanged -= OnStageFail;
        AliveMonsterCountChanged -= OnWarning;
        AliveMonsterCountChanged -= _inGameUi.OnAliveMonsterCountChanged;
    }

    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (CurWave <= _totalWave)
        {
            if (CurWave == 10 || CurWave == 30 || CurWave == 50 || CurWave == 70 || CurWave == 100)
            {
                SpawnMonster(isBoss: true);
            }

            for (int i = 0; i < _monstersPerWave; i++)
            {
                int moveSpeed = _monstersPerWave - i;
                SpawnMonster(isBoss: false, moveSpeed);
                yield return _spawnDelay;
            }

            CurWave++;
            yield return _waveDelay;
        }
    }

    private void OnWaveChanged(int wave) { }

    private void OnStageFail(int count)
    {
        if (count == 50)
        {
            GameManager.Instance.FailStage();
        }
    }

    private void OnWarning(int count)
    {
        if (count == 40)
        {
            UIManager.Instance.ShowPanelTemp("WarningPanel", 3);
        }
    }

    public void OnClearStage(int count)
    {
        if (count >= 1200)
        {
            GameManager.Instance.ClearStage();
        }
    }

    private void SpawnMonster(bool isBoss, int moveSpeed = 1)
    {
        if (isBoss)
        {
            GameObject BossInstance = Instantiate(_bossPrefabs[_toSpawnBossindex], _spawnPoint.position, Quaternion.identity);
            _toSpawnBossindex++;
        }
        else
        {
            PooledObject pooledObject = _objectPool.GetPool(_spawnPoint.position, Quaternion.identity);

            MonsterModel model = pooledObject.GetComponent<MonsterModel>();
            if (model != null)
            {
                model.MoveSpeed = moveSpeed;
            }
        }
        OnMonsterSpawn();
    }

    public void OnMonsterDie()
    {
        AliveMonsterCount = Mathf.Max(0, AliveMonsterCount - 1);
        DeadMonsterCount++;
    }

    public void OnMonsterSpawn()
    {
        SpawnedMonsterCount++;
        AliveMonsterCount++;
    }
}
