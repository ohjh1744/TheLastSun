using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameMainPanel : UIBInder
{
    private enum ENotify { Spawn, MobNum };
    private enum ESpawn { Normal, Special };
    //일반,레어,고대,전설,에픽 신화 -> 전사,궁수,폭탄병순
    private int[,] _spawnIndex = { { 0, 5, 10 }, { 1, 6, 11 }, { 2, 7, 12 }, { 3, 8, 13 }, { 4, 9, 14 }, { 15, 16, 17 } };

    [Header("소환 관련")]
    [SerializeField] private Color[] _greatHeroSpawnTextColors;

    StringBuilder _sb = new StringBuilder();
    private string[] _heroName = { "전사", "궁수", "폭탄병" };
    private string[] _godname = { "토난친", "나나우아틀", "시우테쿠틀리" };
    [SerializeField] private float _setNotifyPaneldurate;
    private Tween _notifySpawnTween;

    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _sfx;
    private AudioClip _spawnClip;

    private float _bgmTime;

    // 몬스터 Info 갖고오기위해서 캐싱
    private SpriteRenderer _mobSR;
    private MobController _mobController;

    [SerializeField] private Color _warnMobColor;
    private Tween _notifyMobNumTween;

    [SerializeField] HeroControlTower _heroControlTower;

    [Header("Set변경해야하는 panel들")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _spawnRatePanel;

    private bool _isSpawnWarrior;
    private bool _isSpawnArcher;
    private bool _isSpawnBomer;

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        SetAllButtons();
    }

    private void Init()
    {
        _spawnClip = _sfx.clip;
        ShowCurrentJem();
        ShowJemForSpawn();
        SetNormalAndSpecialSpawnButton();
        ShowMobInfo();
        AddEvent();
        ShowJemNumForUpgrade(EUpgrade.Warrior);
        ShowJemNumForUpgrade(EUpgrade.Archer);
        ShowJemNumForUpgrade(EUpgrade.Bomer);
    }

    private void AddEvent()
    {
        GetUI<Button>("SpawnButton").onClick.AddListener(() => Spawn(true));
        GetUI<Button>("SpecialSpawnButton").onClick.AddListener(() => Spawn(false));
        GetUI<Button>("PauseButton").onClick.AddListener(DoPause);
        GetUI<Button>("SoundButton").onClick.AddListener(SetSound);
        GetUI<Button>("SoundMuteButton").onClick.AddListener(SetSound);
        GetUI<Button>("TImeSpeedButton").onClick.AddListener(SpeedUpGame);
        GetUI<Button>("GoSellButton").onClick.AddListener(ShowSellPanel);
        GetUI<Button>("ShowSpawnRateButton").onClick.AddListener(ShowSpawnRatePanel);
        GetUI<Button>("WarriorUpgradeButton").onClick.AddListener(() => UpgardeHero(EUpgrade.Warrior));
        GetUI<Button>("ArcherUpgradeButton").onClick.AddListener(() => UpgardeHero(EUpgrade.Archer));
        GetUI<Button>("BomerUpgradeButton").onClick.AddListener(() => UpgardeHero(EUpgrade.Bomer));
        InGameManager.Instance.JemNumOnChanged += SetNormalAndSpecialSpawnButton;
        InGameManager.Instance.JemNumOnChanged += ShowCurrentJem;
        InGameManager.Instance.CurrentWaveTimeOnChanged += ShowTimer;
        InGameManager.Instance.CurrentWaveNumOnChanged += ShowWaveNum;
        InGameManager.Instance.CurrentWaveNumOnChanged += ShowWarnBossText;
        InGameManager.Instance.CurrentWaveNumOnChanged += ShowMobInfo;
        ObjectPoolManager.Instance.MobNumOnChanged += SHowMobNum;
    }

    private void ShowWarnBossText()
    {
        if (InGameManager.Instance.WaveNum == 24 || InGameManager.Instance.WaveNum == 49)
        {
            _sb.Clear();
            _sb.Append($"제한 시간 내에 {_mobController?.MobData.Name}를 처치해야 합니다!");
            GetUI<TextMeshProUGUI>("WarnBossText").SetText(_sb);
            GetUI("WarnBossText").SetActive(true);
        }
        else
        {
            GetUI("WarnBossText").SetActive(false);
        }
    }

    private void Spawn(bool isNormalSpawn)
    {
        if (InGameManager.Instance.JemNum <= 0)
        {
            Debug.Log("젬없음");
            return;
        }
        //0. 사운드 
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            _sfx.PlayOneShot(_spawnClip);
        }

        //1. 일반스폰확률, 스페셜스폰확률 결정 및 보석 소모
        float[] spawnRates = isNormalSpawn ? InGameManager.Instance.NormalSpawnRates : InGameManager.Instance.SpecialSpawnRates;
        InGameManager.Instance.JemNum -= isNormalSpawn ? InGameManager.Instance.NormalSpawnForJemNum : InGameManager.Instance.SpecialSpawnForJemNum;

        //2.스폰확률에 따라 소환할 등급결정
        float randomGradeValue = Random.Range(0f, 100f);
        int spawnGradeIndex = 0;
        float maxValue = spawnRates[0];

        //확률에 따라 비교 시작
        for (int i = 0; i < spawnRates.Length; i++)
        {
            //랜덤값이 해당확률 내라면 소환할 등급 결정 후 종료
            if (randomGradeValue < maxValue)
            {
                //특수소환 실패 알림 띄우고 return
                if (i == 0 && isNormalSpawn == false)
                {
                    Debug.Log("hi");
                    NotifySpawnFail();
                    return;
                }
                spawnGradeIndex = isNormalSpawn ? i : i + 2;
                Debug.Log($"{maxValue}: {randomGradeValue}{(EHeroGrade)(isNormalSpawn ? i : i + 2)}등급 {(isNormalSpawn ? "소환" : "특수소환")}뽑힘 {spawnGradeIndex}");
                break;
            }
            //max값 누적
            if (i < spawnRates.Length - 1)
            {
                maxValue += spawnRates[i + 1];
            }
        }

        //3. 해당 등급 중에 1/3확률로 유닛 뽑기, 뽑고나서 마리수 증가
        int randomHeroValue = Random.Range(0, 3);
        GameObject hero = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.Instance.HeroPools, ObjectPoolManager.Instance.Heros, _spawnIndex[spawnGradeIndex, randomHeroValue]);
        hero.transform.position = Vector3.zero;
        _heroControlTower.OnHeroActivated(hero, (EHeroPool)_spawnIndex[spawnGradeIndex, randomHeroValue]);

        //4. 유닛 마리수 증가
        ObjectPoolManager.Instance.SetHeroNum(_spawnIndex[spawnGradeIndex, randomHeroValue], ObjectPoolManager.Instance.GetHeroNum(_spawnIndex[spawnGradeIndex, randomHeroValue]) + 1);

        //해당 직업 소환됐는지 확인
        if (_spawnIndex[spawnGradeIndex, randomHeroValue] >= (int)EHeroPool.N_Warrior && _spawnIndex[spawnGradeIndex, randomHeroValue] <= (int)EHeroPool.E_Warrior)
        {
            _isSpawnWarrior = true;
        }
        else if (_spawnIndex[spawnGradeIndex, randomHeroValue] >= (int)EHeroPool.N_Archer && _spawnIndex[spawnGradeIndex, randomHeroValue] <= (int)EHeroPool.E_Archer)
        {
            _isSpawnArcher = true;
        }
        else if (_spawnIndex[spawnGradeIndex, randomHeroValue] >= (int)EHeroPool.N_Bomer && _spawnIndex[spawnGradeIndex, randomHeroValue] <= (int)EHeroPool.E_Bomer)
        {
            _isSpawnBomer = true;
        }

        //5. 전설 등급 이상은 알림켜주기
        if (spawnGradeIndex >= (int)EHeroGrade.Legend)
        {
            NotifySpawnUnit(spawnGradeIndex, randomHeroValue);
        }
    }

    private void NotifySpawnFail()
    {
        _sb.Clear();
        _sb.Append($"특수소환에 실패하였습니다.");
        GetUI<TextMeshProUGUI>("NotifyText").SetText(_sb);
        GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[0];
        TurnSetNotifyPanel(ENotify.Spawn);
    }

    private void NotifySpawnUnit(int heroGradeIndex, int hero)
    {
        _sb.Clear();
        if (heroGradeIndex == (int)EHeroGrade.Legend)
        {
            _sb.Append($"전설의 아즈텍 {_heroName[hero]}가 등장합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.Legend - 2];
        }
        else if (heroGradeIndex == (int)EHeroGrade.Epic)
        {
            _sb.Append($"드높은 긍지의 아즈텍 {_heroName[hero]}가 등장합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.Epic - 2];
        }
        else if (heroGradeIndex == (int)EHeroGrade.God)
        {
            _sb.Append($"인간을 구원해줄 {_godname[hero]}가 강림합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.God - 2];
        }
        GetUI<TextMeshProUGUI>("NotifyText").SetText(_sb);
        TurnSetNotifyPanel(ENotify.Spawn);
    }

    private void TurnSetNotifyPanel(ENotify eNotify)
    {
        GameObject _notifyPanel = (eNotify == ENotify.Spawn) ? GetUI("NotifyPanel") : GetUI("NotifyMobNumPanel");
        _notifyPanel.SetActive(true);

        Tween _tween = (eNotify == ENotify.Spawn) ? _notifySpawnTween : _notifyMobNumTween;

        _tween?.Kill();

        // 3초 후 비활성화
        _tween = DOVirtual.DelayedCall(_setNotifyPaneldurate, () =>
        {
            _notifyPanel.SetActive(false);
        });
    }

    private void ShowCurrentJem()
    {
        _sb.Clear();
        _sb.Append(InGameManager.Instance.JemNum);
        GetUI<TextMeshProUGUI>("ShowJemText").SetText(_sb);
    }

    private void SetNormalAndSpecialSpawnButton()
    {
        if (InGameManager.Instance.JemNum < InGameManager.Instance.NormalSpawnForJemNum)
        {
            GetUI<Button>("SpawnButton").interactable = false;
            GetUI<Button>("SpecialSpawnButton").interactable = false;
        }
        else if (InGameManager.Instance.JemNum >= InGameManager.Instance.NormalSpawnForJemNum && InGameManager.Instance.JemNum < InGameManager.Instance.SpecialSpawnForJemNum)
        {
            GetUI<Button>("SpawnButton").interactable = true;
            GetUI<Button>("SpecialSpawnButton").interactable = false;
        }
        else
        {
            GetUI<Button>("SpawnButton").interactable = true;
            GetUI<Button>("SpecialSpawnButton").interactable = true;
        }
    }

    private void ShowJemForSpawn()
    {
        _sb.Clear();
        _sb.Append(InGameManager.Instance.NormalSpawnForJemNum);
        GetUI<TextMeshProUGUI>("SpawnButtonJemNumText").SetText(_sb);

        _sb.Clear();
        _sb.Append(InGameManager.Instance.SpecialSpawnForJemNum);
        GetUI<TextMeshProUGUI>("SpecialSpawnJemNumText").SetText(_sb);
    }


    private void ShowTimer()
    {
        _sb.Clear();
        int minutes = Mathf.FloorToInt(InGameManager.Instance.CurrentWaveTime / 60f);
        int seconds = Mathf.FloorToInt(InGameManager.Instance.CurrentWaveTime % 60f);

        _sb.AppendFormat("{0:00}:{1:00}", minutes, seconds);
        GetUI<TextMeshProUGUI>("WaveInfoTimerText").SetText(_sb);
    }

    private void ShowMobInfo()
    {
        // 1. 몬스터 이미지
        _mobSR = ObjectPoolManager.Instance.Mobs[InGameManager.Instance.WaveNum].GetComponent<SpriteRenderer>();
        GetUI<Image>("WaveInfoMobImage").sprite = _mobSR.sprite;
        GetUI<Image>("WaveInfoMobImage").color = _mobSR.color;

        //2.몬스터 네임
        _mobController = ObjectPoolManager.Instance.Mobs[InGameManager.Instance.WaveNum].GetComponent<MobController>();
        _sb.Clear();
        _sb.Append(_mobController.MobData.Name);
        GetUI<TextMeshProUGUI>("WaveInfoMobNameText").SetText(_sb);
    }

    private void SHowMobNum()
    {
        //텍스트 및 슬라이더 표시
        _sb.Clear();
        _sb.Append($"{ObjectPoolManager.Instance.MobNum} / {InGameManager.Instance.MobNumForDefeat}");
        GetUI<TextMeshProUGUI>("MobNumText").SetText(_sb);

        GetUI<Slider>("MobNumSlider").value = (float)ObjectPoolManager.Instance.MobNum / (float)InGameManager.Instance.MobNumForDefeat;

        //경고 알람 띄우기
        for (int i = 0; i < InGameManager.Instance.MobNumForDefeatWarning.Length; i++)
        {
            if (ObjectPoolManager.Instance.MobNum == InGameManager.Instance.MobNumForDefeatWarning[i])
            {
                _sb.Clear();
                _sb.Append($"현재 몬스터가 {ObjectPoolManager.Instance.MobNum}가 넘습니다.");
                GetUI<TextMeshProUGUI>("NotifyMobNumText").SetText(_sb);
                GetUI<TextMeshProUGUI>("NotifyMobNumText").color = _warnMobColor;
                TurnSetNotifyPanel(ENotify.MobNum);
            }
        }
    }

    private void ShowWaveNum()
    {
        _sb.Clear();
        _sb.Append($"Wave {InGameManager.Instance.WaveNum + 1}");
        GetUI<TextMeshProUGUI>("WaveInfoIndexText").SetText(_sb);
    }

    private void DoPause()
    {
        _pausePanel.SetActive(true);
    }


    private void SetSound()
    {
        if (NetworkCheckManager.Instance.IsConnected == true)
        {
            if (PlayerController.Instance.PlayerData.IsSound == true)
            {
                PlayerController.Instance.PlayerData.IsSound = false;
                GpgsManager.Instance.SaveData((success) => { });
                Debug.Log($"인게임 사운드 바꿈{PlayerController.Instance.PlayerData.IsSound}");
                GetUI("SoundButton").SetActive(false);
                GetUI("SoundMuteButton").SetActive(true);
                _bgm.Pause();
                _bgmTime = _bgm.time;
            }
            else if (PlayerController.Instance.PlayerData.IsSound == false)
            {
                PlayerController.Instance.PlayerData.IsSound = true;
                GpgsManager.Instance.SaveData((success) => { });
                Debug.Log($"인게임 사운드 바꿈{PlayerController.Instance.PlayerData.IsSound}");
                GetUI("SoundButton").SetActive(true);
                GetUI("SoundMuteButton").SetActive(false);
                _bgm.time = _bgmTime;
                _bgm.Play();
            }
        }
    }

    private void SpeedUpGame()
    {
        _sb.Clear();
        InGameManager.Instance.SpeedUpIndex = (InGameManager.Instance.SpeedUpIndex + 1) % InGameManager.Instance.SpeedUpRate.Length;
        Debug.Log(InGameManager.Instance.SpeedUpIndex);
        _sb.Append($"x {InGameManager.Instance.SpeedUpRate[InGameManager.Instance.SpeedUpIndex]}");
        GetUI<TextMeshProUGUI>("TimeSpeedButtonText").SetText(_sb);
    }

    private void ShowSellPanel()
    {
        GetUI("SellPanel").SetActive(true);
    }

    private void ShowSpawnRatePanel()
    {
        _spawnRatePanel.SetActive(true);
    }

    private void SetAllButtons()
    {
        //설정패널 or 네트워크에러패널이 뜨는 경우
        if (InGameManager.Instance.GameState == EGameState.Win || InGameManager.Instance.GameState == EGameState.Defeat || InGameManager.Instance.GameState == EGameState.Pause || NetworkCheckManager.Instance.IsConnected == false)
        {
            GetUI<Button>("PauseButton").interactable = false;
            GetUI<Button>("SoundButton").interactable = false;
            GetUI<Button>("SoundMuteButton").interactable = false;
            GetUI<Button>("TImeSpeedButton").interactable = false;
            GetUI<Button>("SpawnButton").interactable = false;
            GetUI<Button>("SpecialSpawnButton").interactable = false;
            GetUI<Button>("ShowSpawnRateButton").interactable = false;
            GetUI<Button>("GoSellButton").interactable = false;
            GetUI<Button>("WarriorUpgradeButton").interactable = false;
            GetUI<Button>("ArcherUpgradeButton").interactable = false;
            GetUI<Button>("BomerUpgradeButton").interactable = false;
        }
        // 설정패널 or 네트워크에러창이 꺼진 경우
        else if (InGameManager.Instance.GameState == EGameState.Play || NetworkCheckManager.Instance.IsConnected == true)
        {
            GetUI<Button>("PauseButton").interactable = true;
            GetUI<Button>("SoundButton").interactable = true;
            GetUI<Button>("SoundMuteButton").interactable = true;
            GetUI<Button>("TImeSpeedButton").interactable = true;
            SetNormalAndSpecialSpawnButton();
            GetUI<Button>("ShowSpawnRateButton").interactable = true;
            GetUI<Button>("GoSellButton").interactable = true;
            SetUpgradeButton();
        }
    }

    // To Do: 강화
    private void ShowJemNumForUpgrade(EUpgrade _upgradeHero)
    {
        switch (_upgradeHero)
        {
            case EUpgrade.Warrior:
                _sb.Clear();
                _sb.Append(InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero]);
                GetUI<TextMeshProUGUI>("ShowWarriorUpgradeInfoText").SetText(_sb);
                break;
            case EUpgrade.Archer:
                _sb.Clear();
                _sb.Append(InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero]);
                GetUI<TextMeshProUGUI>("ShowArcherUpgradeInfoJemText").SetText(_sb);
                break;
            case EUpgrade.Bomer:
                _sb.Clear();
                _sb.Append(InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero]);
                GetUI<TextMeshProUGUI>("ShowBomerUpgradeInfoJemText").SetText(_sb);
                break;
        }
    }

    private void SetUpgradeButton()
    {
        if(InGameManager.Instance.JemNum < InGameManager.Instance.JemNumsForUpgrade[(int)EUpgrade.Warrior] || _isSpawnWarrior == false)
        {
            GetUI<Button>("WarriorUpgradeButton").interactable = false;
        }
        else
        {
            GetUI<Button>("WarriorUpgradeButton").interactable = true;
        }

        if (InGameManager.Instance.JemNum < InGameManager.Instance.JemNumsForUpgrade[(int)EUpgrade.Archer] || _isSpawnArcher == false)
        {
            GetUI<Button>("ArcherUpgradeButton").interactable = false;
        }
        else
        {
            GetUI<Button>("ArcherUpgradeButton").interactable = true;
        }

        if (InGameManager.Instance.JemNum < InGameManager.Instance.JemNumsForUpgrade[(int)EUpgrade.Bomer] || _isSpawnBomer == false)
        {
            GetUI<Button>("BomerUpgradeButton").interactable = false;
        }
        else
        {
            GetUI<Button>("BomerUpgradeButton").interactable = true;
        }
    }

    private void UpgardeHero(EUpgrade _upgradeHero)
    {
        if (InGameManager.Instance.JemNum < InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero])
        {
            return;
        }

        //사운드
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            _sfx.PlayOneShot(_spawnClip);
        }

        //젬 개수 감소
        InGameManager.Instance.JemNum -= InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero];
        //강화 젬 개수 증가
        InGameManager.Instance.JemNumsForUpgrade[(int)_upgradeHero] += InGameManager.Instance.JemNumPlusForUpgrade[(int)_upgradeHero];
        //레벨 증가
        InGameManager.Instance.UpgradeLevels[(int)_upgradeHero]++;

        //업그레이드에 필요한 젬 개수 및 Level 표시
        switch (_upgradeHero)
        {
            case EUpgrade.Warrior:
                ShowJemNumForUpgrade(EUpgrade.Warrior);
                _sb.Clear();
                _sb.Append($"Lv {InGameManager.Instance.UpgradeLevels[(int)_upgradeHero] + 1}");
                GetUI<TextMeshProUGUI>("WarriorUpgradeButtonText").SetText(_sb);
                break;
            case EUpgrade.Archer:
                ShowJemNumForUpgrade(EUpgrade.Archer);
                _sb.Clear();
                _sb.Append($"Lv {InGameManager.Instance.UpgradeLevels[(int)_upgradeHero] + 1}");
                GetUI<TextMeshProUGUI>("ArcherUpgradeButtonText").SetText(_sb);
                break;
            case EUpgrade.Bomer:
                ShowJemNumForUpgrade(EUpgrade.Bomer);
                _sb.Clear();
                _sb.Append($"Lv {InGameManager.Instance.UpgradeLevels[(int)_upgradeHero] + 1}");
                GetUI<TextMeshProUGUI>("BomerUpgradeButtonText").SetText(_sb);
                break;
        }
    }

    #region 주석처리
    //[ContextMenu("FillSPrites")]
    //private void FillStageMobSprites()
    //{
    //    //_mob2Sprites.Clear();

    //    for (int i = 1; i <= 49; i++)
    //    {
    //        string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage2/Stage2_Mob_{i}.prefab";
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
    //        if (!prefab)
    //        {
    //            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {assetPath}");
    //            continue;
    //        }

    //        // SpriteRenderer에서 Sprite 가져오기
    //        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
    //        if (!sr || !sr.sprite)
    //        {
    //            Debug.LogWarning($"{prefab.name}에 SpriteRenderer 또는 Sprite가 없습니다.");
    //            continue;
    //        }

    //        // Sprite의 GUID를 가져와 AssetReferenceSprite 생성
    //        string spritePath = AssetDatabase.GetAssetPath(sr.sprite);
    //        string guid = AssetDatabase.AssetPathToGUID(spritePath);

    //        AssetReferenceSprite reference = new AssetReferenceSprite(guid);
    //        //_mob2Sprites.Add(reference);
    //    }

    //    EditorUtility.SetDirty(this);
    //    //Debug.Log($"{_mob2Sprites.Count}개의 Stage2 Sprite 참조를 생성했습니다.");
    //}

    //[ContextMenu("Fill Colors")]
    //private void FillStageMobColors()
    //{
    //    for (int i = 1; i <= 49; i++)
    //    {
    //        string assetPath = $"Assets/Prefabs/OJH/Monsters/Stage5/Stage5_Mob_{i}.prefab";
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

    //        if (prefab == null)
    //        {
    //            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {assetPath}");
    //            continue;
    //        }

    //        // SpriteRenderer에서 색상 가져오기
    //        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
    //        if (sr == null)
    //        {
    //            Debug.LogWarning($"{prefab.name}에서 SpriteRenderer를 찾을 수 없습니다.");
    //            continue;
    //        }

    //        Color color = sr.color;
    //       // _savedStage5MobColors.Add(color);
    //    }

    //    EditorUtility.SetDirty(this);
    //   // Debug.Log($"{_savedStage5MobColors.Count}개의 색상을 성공적으로 추가했습니다!");
    //}
    #endregion
}
