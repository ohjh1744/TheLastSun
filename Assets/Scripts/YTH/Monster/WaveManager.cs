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
    public int AliveMonsterCount { get=> _aliveMonsterCount; set { _aliveMonsterCount = value; AliveMonsterCountChanged?.Invoke(_aliveMonsterCount); } }

    [SerializeField] int _deaddMonsterCount = 0;
    public int DeaddMonsterCount {
        get => _deaddMonsterCount;
        set { _deaddMonsterCount = value; if (_deaddMonsterCount >= _totalWave * _monstersPerWave) { ClearStage?.Invoke(); }} }

    private int _curWave = 1;
    public int CurWave { get => _curWave; private set { _curWave = value; CurWaveChanged?.Invoke(_curWave); } }

    [Header("Set")]
    [SerializeField] int _totalWave = 100;
    [SerializeField] int _monstersPerWave = 12;
    [SerializeField] Transform _spawnPoint;
    [HideInInspector] public Transform SpawnPoint => _spawnPoint;
    [SerializeField] List<GameObject> _bossPrefabs;

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(6f);
    private WaitForSeconds _waveDelay = new(3f);

    // 이벤트 선언
    public event Action<int> SpawnedMonsterCountChanged;
    public event Action<int> AliveMonsterCountChanged; //TODO: 살아있는 몬스터 수 변경이 제대로 이루어지지 않음; 마이너스가 됨 수정필요 
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
        // 이벤트 구독
        CurWaveChanged += OnWaveChanged;
        AliveMonsterCountChanged += OnStageFail;
        AliveMonsterCountChanged += OnWarning;
        SpawnedMonsterCountChanged += OnWarning;
        SpawnedMonsterCountChanged += _inGameUi.OnAliveMonsterCountChanged;
        ClearStage += OnClearStage;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        CurWaveChanged -= OnWaveChanged;
        AliveMonsterCountChanged -= OnStageFail;
        AliveMonsterCountChanged -= OnWarning;
        SpawnedMonsterCountChanged -= OnWarning;
        SpawnedMonsterCountChanged -= _inGameUi.OnAliveMonsterCountChanged;
        ClearStage -= OnClearStage;
    }

    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (CurWave <= _totalWave)
        {
            Debug.Log($"CurWave: {CurWave}, 보스 체크: {CurWave == 10 || CurWave == 30 || CurWave == 50 || CurWave == 70 || CurWave == 100}");
            // 보스 웨이브 체크 및 보스 소환
            if (CurWave == 10 || CurWave == 30 || CurWave == 50 || CurWave == 70 || CurWave == 100)
            {
                SpawnMonster(isBoss: true);
            }

            for (int i = 0; i < _monstersPerWave; i++)
            {
                int moveSpeed = _monstersPerWave - i; // 12, 11, ..., 1
                SpawnMonster(isBoss: false, moveSpeed);
                yield return _spawnDelay;
            }

            CurWave++;
            yield return _waveDelay; // 웨이브 간 대기
        }
    }

    // OnWaveChanged는 UI 등 외부 구독용으로만 사용
    private void OnWaveChanged(int wave)
    {
        // 필요시 UI 갱신 등만 처리
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
            UIManager.Instance.ShowPanelTemp("WarningPanel", 3);
        }
    }

    private void OnClearStage()
    {
        GameManager.Instance.ClearStage();
    }

    private void SpawnMonster(bool isBoss, int moveSpeed = 1)
    {
        if (isBoss)
        {
            // 보스 몬스터 소환
            GameObject BossInstance = Instantiate(_bossPrefabs[_toSpawnBossindex], _spawnPoint.position, Quaternion.identity);
            _toSpawnBossindex++;
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
        OnMonsterSpawn();
      
    }

    // 몬스터가 죽을 때(예시)
    public void OnMonsterDie()
    {
        AliveMonsterCount--;
        DeaddMonsterCount++;
    }

    public void OnMonsterSpawn()
    {
        SpawnedMonsterCount++;
        AliveMonsterCount++;
    }
}
