using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] float timer;
    public static WaveManager Instance { get; private set; }

    private InGameUI _inGameUi;

    [Header("Don't Set")]
    [SerializeField] int _toSpawnBossindex = 0;
    public int ToSpawnBossindex { get => _toSpawnBossindex; set { _toSpawnBossindex = value; OnChangeBoss?.Invoke(); } }

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
    [SerializeField] int _totalWave = 50;
    [SerializeField] int _monstersPerWave = 12;
    [SerializeField] int _spawnDelaySeconds;
    private int _clearCondition => _totalWave * _monstersPerWave;
    public int ClearCondition => _clearCondition;

    [SerializeField] Transform _spawnPoint;
    [HideInInspector] public Transform SpawnPoint => _spawnPoint;
    [SerializeField] float[] _waveTerm;
    [SerializeField] List<string> _bossMonsterName;
    [SerializeField] List<string> _chapter1_normalMonsterName;
    [SerializeField] List<string> _chapter2_normalMonsterName;
    [SerializeField] List<string> _chapter3_normalMonsterName;
    [SerializeField] List<string> _chapter4_normalMonsterName;
    [SerializeField] List<string> _chapter5_normalMonsterName;
    public List<string> BossMonsterName { get => _bossMonsterName; set { _bossMonsterName = value; OnChangeBoss?.Invoke(); } }

    private ObjectPool _objectPool;

    private WaitForSeconds _spawnDelay = new(1.5f);
    private WaitForSeconds _waveDelay = new(10f);


    public event Action<int> SpawnedMonsterCountChanged;
    public event Action<int> AliveMonsterCountChanged;
    public event Action<int> CurWaveChanged;
    public event Action ClearStage;
    public event Action OnChangeBoss;

    bool _isStarted = false;

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
        /*StartCoroutine(WaveRoutine());*/
    }

    private void Update()
    {
        if (_isStarted)
        {
            timer += Time.deltaTime;
        }
    }

    public void StartWave()
    {
        StartCoroutine(WaveRoutine());
        _isStarted = true;
    }

    public IEnumerator WaveRoutine()
    {
        while (CurWave <= _totalWave)
        {
            int wave = CurWave; // 현재 웨이브 캡처
            int MoveSpeed = CurWave % 2 == 0 ? 2 : 1;

            bool isBossWave = (wave == 50);
            if (isBossWave)
            {
                // 보스 1회 소환
                SpawnMonster(isBoss: true);

                // 현재 웨이브의 텀만큼 기다림
                yield return new WaitUntil(() => timer >= GetWaveTerm(wave));
                timer = 0f;

                CurWave++;
                continue;
            }
            else
            {
                if (wave == 25)
                {
                    SpawnMonster(isBoss: false, MoveSpeed);
                    yield return new WaitForSeconds(_spawnDelaySeconds);
                }
                else
                {
                    for (int i = 0; i < _monstersPerWave; i++)
                    {
                        SpawnMonster(isBoss: false, MoveSpeed);
                        yield return new WaitForSeconds(_spawnDelaySeconds);
                    }
                }

                // 현재 웨이브의 텀만큼 기다림
                yield return new WaitUntil(() => timer >= GetWaveTerm(wave));
                timer = 0f;

                CurWave++;
                continue;
            }
        }
    }

    private float GetWaveTerm(int wave)
    {
        if (_waveTerm == null || _waveTerm.Length == 0)
            return 10f; // 기본 대기시간

        int idx = Mathf.Clamp(wave - 1, 0, _waveTerm.Length - 1);
        return Mathf.Max(0f, _waveTerm[idx]);
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
        if (count >= ClearCondition)
        {
            GameManager.Instance.ClearStage();
        }
    }

    private void SpawnMonster(bool isBoss, int moveSpeed = 1)
    {
        int curStage = PlayerController.Instance.PlayerData.CurrentStage;
        string address = isBoss
            ? $"Assets/Prefabs/OJH/Monsters/Boss/Stage{curStage + 1}_Boss.prefab"
            : $"Assets/Prefabs/OJH/Monsters/Stage{curStage + 1}/Stage{curStage + 1}_Mob_{CurWave}.prefab";

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

                OnMonsterSpawn();
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

                var mover = obj.GetComponent<MonsterMover>();
                if (mover != null)
                    mover.RestartMove();

                OnMonsterSpawn();
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
