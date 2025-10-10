using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private InGameUI _inGameUi;

    [Header("Don't Set")]
    public int ToSpawnBossindex;

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
    public List<GameObject> BossPrefabs { get => _bossPrefabs; set { _bossPrefabs = value; OnChangeBoss?.Invoke(); } }

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(1.5f);
    private WaitForSeconds _waveDelay = new(10f);

    public event Action<int> SpawnedMonsterCountChanged;
    public event Action<int> AliveMonsterCountChanged;
    public event Action<int> CurWaveChanged;
    public event Action ClearStage;
    public event Action OnChangeBoss;

    // 몬스터 주소 인덱스 누적 관리용 필드
    int nextMonsterIndex = 1;

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
        AliveMonsterCountChanged += OnStageFail;
        AliveMonsterCountChanged += OnWarning;
        AliveMonsterCountChanged += OnWarning2;
        AliveMonsterCountChanged += _inGameUi.OnAliveMonsterCountChanged;
    }

    private void OnDisable()
    {
        AliveMonsterCountChanged -= OnStageFail;
        AliveMonsterCountChanged -= OnWarning;
        AliveMonsterCountChanged -= OnWarning2;
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

    private void OnStageFail(int count)
    {
        if (count == 51)
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

    private void OnWarning2(int count)
    {
        if (count == 50)
        {
            UIManager.Instance.ShowPanelTemp("WarningPanel2", 3);
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
        int curStage = 1;
        string address = isBoss
            ? $"Assets/Prefabs/OJH/Monsters/Boss/Stage{curStage}_Boss.prefab"
            : $"Assets/Prefabs/OJH/Monsters/Stage{curStage}/Stage{curStage}_Mob_{nextMonsterIndex}.prefab";

        if (isBoss)
        {
            AddressableManager.Instance.GetObject(address, transform, (obj) =>
            {
                if (obj == null)
                {
                    Debug.LogError($"[WaveManager] Boss Spawn 실패 - address: {address}");
                    return;
                }

                Debug.Log($"[WaveManager] Boss Spawn - address: {address}");
                obj.transform.position = _spawnPoint.position;
                obj.SetActive(true);

                OnMonsterSpawn(); // 성공 시에만 카운트
                ToSpawnBossindex++;
            });
        }
        else
        {
            AddressableManager.Instance.GetObject(address, transform, (obj) =>
            {
                if (obj == null)
                {
                    Debug.LogError($"[WaveManager] Mob Spawn 실패 - address: {address}");
                    return;
                }

                obj.transform.position = _spawnPoint.position;
                obj.SetActive(true);

                MonsterModel model = obj.GetComponent<MonsterModel>();
                if (model != null)
                {
                    model.MoveSpeed = moveSpeed;
                }

                OnMonsterSpawn();   // 성공 시에만 카운트
                nextMonsterIndex = (nextMonsterIndex < 49) ? nextMonsterIndex + 1 : 1; // 성공 시에만 인덱스 증가
            });
        }
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
