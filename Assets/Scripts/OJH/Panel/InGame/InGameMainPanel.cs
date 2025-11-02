using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InGameMainPanel : UIBInder
{
    private enum ESpawn{Normal, Special};
    //일반,레어,고대,전설,에픽 신화 -> 전사,궁수,폭탄병순
    private int[,] _spawnIndex = { { 0, 5, 10 }, { 1, 6, 11 }, { 2, 7, 12 }, { 3, 8, 13 }, { 4, 9, 14 }, { 15, 16, 17 } };

    [Header("소환 관련")]
    [SerializeField] private Color[] _greatHeroSpawnTextColors;

    StringBuilder _sb = new StringBuilder();
    private  string[] _heroName = { "전사", "궁수", "폭탄병" };
    private string[] _godname = { "토난친", "나나우아틀", "시우테쿠틀리" };
    [SerializeField] private float _setNotifyPaneldurate;
    private Tween _notifyTween;

    [SerializeField] private AudioSource _sfx;
    private AudioClip _spawnClip;
 
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
        ShowSpawnJem();
        SetNormalAndSpecialSpawnButton();
        AddEvent();
    }

    private void AddEvent()
    {
        GetUI<Button>("SpawnButton").onClick.AddListener(() => Spawn(true));
        GetUI<Button>("SpecialSpawnButton").onClick.AddListener(() => Spawn(false));
        GetUI<Button>("PauseButton").onClick.AddListener(DoPause);
        GetUI<Button>("TImeSpeedButton").onClick.AddListener(SpeedUpGame);
        GetUI<Button>("GoSellButton").onClick.AddListener(ShowSellPanel);
        InGameManager.Instance.JemNumOnChanged += SetNormalAndSpecialSpawnButton;
        InGameManager.Instance.JemNumOnChanged += ShowCurrentJem;
        InGameManager.Instance.CurrentWaveTimeOnChanged += ShowTimer;
        InGameManager.Instance.CurrentWaveNumOnChanged += ShowWaveNum;
    }

    private void Spawn(bool isNormalSpawn)
    {
        _sfx.PlayOneShot(_spawnClip);
        //1. 일반스폰확률, 스페셜스폰확률 결정 및 보석 소모
        float[] spawnRates;
        if (isNormalSpawn == true)
        {
            InGameManager.Instance.JemNum -= InGameManager.Instance.NormalSpawnForJemNum;
            Debug.Log(InGameManager.Instance.JemNum);
            spawnRates = InGameManager.Instance.NormalSpawnRates;
        }
        else
        {
            InGameManager.Instance.JemNum -= InGameManager.Instance.SpecialSpawnForJemNum;
            spawnRates = InGameManager.Instance.SpecialSpawnRates;
        }

        //2.스폰확률에 따라 소환할 등급결정
        float randomGradeValue = Random.Range(0f, 100f);
        int spawnGradeIndex = 0;
        float maxValue =  spawnRates[0]-1;
        for (int i = 0; i < spawnRates.Length; i++)
        {
            Debug.Log(maxValue);
            if (randomGradeValue < maxValue)
            {
                //특수소환 실패라면
                if(i == 0 && isNormalSpawn == false)
                {
                    //특수소환 실패 알림 띄우고 return
                    Debug.Log($"{maxValue}: {randomGradeValue}특수소환 실패");
                    return;
                }
                else
                {
                    // 해당 등급 뽑기 결졍
                    //특수소환의 경우 인덱스 2차이나기에 
                    if(isNormalSpawn == false)
                    {
                        spawnGradeIndex = i + 2;
                        Debug.Log($"{maxValue}: {randomGradeValue}{(EHeroGrade)i+2}등급 특수소환뽑힘");
                    }
                    else
                    {
                        spawnGradeIndex = i;
                        Debug.Log($"{maxValue}: {randomGradeValue}{(EHeroGrade)i}등급 소환뽑힘");
                    }
                    break;
                }
            }
            if(i < spawnRates.Length - 1)
            {
                maxValue = maxValue + spawnRates[i + 1];
            }
        }

        //3. 해당 등급 중에 1/3확률로 유닛 뽑기, 뽑고나서 마리수 증가
        int randomHeroValue = Random.Range(0, 3);
        GameObject hero = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.Instance.HeroPools, ObjectPoolManager.Instance.Heros, _spawnIndex[spawnGradeIndex, randomHeroValue]);
        hero.transform.position = Vector3.zero;
        ObjectPoolManager.Instance.SetHeroNum(_spawnIndex[spawnGradeIndex, randomHeroValue], ObjectPoolManager.Instance.GetHeroNum(_spawnIndex[spawnGradeIndex, randomHeroValue]) + 1);
        Debug.Log((EHeroPool)_spawnIndex[spawnGradeIndex, randomHeroValue]);
        //4. 전설 등급 이상은 알림켜주기
        if (spawnGradeIndex >= (int)EHeroGrade.Legend)
        {
            NotifySpawnGreatUnit(spawnGradeIndex, randomHeroValue);
        }
    }

    private void NotifySpawnGreatUnit(int heroGradeIndex, int hero)
    {
        _sb.Clear();
        if(heroGradeIndex == (int)EHeroGrade.Legend)
        {
            _sb.Append($"전설의 아즈텍 {_heroName[hero]}가 등장합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.Legend - 3];
        }
        else if (heroGradeIndex == (int)EHeroGrade.Epic)
        {
            _sb.Append($"드높은 긍지의 아즈텍 {_heroName[hero]}가 등장합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.Epic - 3];
        }
        else if (heroGradeIndex == (int)EHeroGrade.God)
        {
            _sb.Append($"인간을 구원해줄 {_godname[hero]}가 강림합니다!");
            GetUI<TextMeshProUGUI>("NotifyText").color = _greatHeroSpawnTextColors[(int)EHeroGrade.God - 3];
        }
        GetUI<TextMeshProUGUI>("NotifyText").SetText(_sb);
        TurnSetNotifyPanel();
    }

    private void TurnSetNotifyPanel()
    {
        GetUI("NotifyPanel").SetActive(true);

        _notifyTween?.Kill();

        // 3초 후 비활성화
        _notifyTween = DOVirtual.DelayedCall(_setNotifyPaneldurate, () =>
        {
            GetUI("NotifyPanel").SetActive(false);
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
        if(InGameManager.Instance.JemNum < InGameManager.Instance.NormalSpawnForJemNum)
        {
            GetUI<Button>("SpawnButton").interactable = false;
            GetUI<Button>("SpecialSpawnButton").interactable = false;
        }
        else if (InGameManager.Instance.JemNum < InGameManager.Instance.SpecialSpawnForJemNum)
        {
            GetUI<Button>("SpecialSpawnButton").interactable = false;
        }
        else
        {
            GetUI<Button>("SpawnButton").interactable = true;
            GetUI<Button>("SpecialSpawnButton").interactable = true;
        }
    }

    private void ShowSpawnJem()
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

    private void ShowWaveNum()
    {
        _sb.Clear();
        _sb.Append($"Wave {InGameManager.Instance.WaveNum + 1}");
        GetUI<TextMeshProUGUI>("WaveInfoIndexText").SetText(_sb);
    }

    private void DoPause()
    {
        InGameManager.Instance.GameState = EGameState.Pause;
        GetUI("PausePanel").gameObject.SetActive(true);
    }

    private void SpeedUpGame()
    {
        _sb.Clear();
        InGameManager.Instance.SpeedUpIndex = (InGameManager.Instance.SpeedUpIndex + 1)% InGameManager.Instance.SpeedUpRate.Length;
        Debug.Log(InGameManager.Instance.SpeedUpIndex);
        _sb.Append($"x {InGameManager.Instance.SpeedUpRate[InGameManager.Instance.SpeedUpIndex]}");
        GetUI<TextMeshProUGUI>("TimeSpeedButtonText").SetText(_sb);
    }

    private void ShowSellPanel()
    {
        GetUI("SellPanel").SetActive(true);
    }

    private void SetAllButtons()
    {
        //설정패널 or 네트워크에러패널이 뜨는 경우
        if (GetUI("ClearPanel").activeSelf == true || GetUI("PausePanel").activeSelf == true || NetworkCheckManager.Instance.IsConnected == false)
        {
            //mainPanel
            GetUI<Button>("PauseButton").interactable = false;
            GetUI<Button>("SoundButton").interactable = false;
            GetUI<Button>("TImeSpeedButton").interactable = false;
            GetUI<Button>("SpawnButton").interactable = false;
            GetUI<Button>("SpecialSpawnButton").interactable = false;
            GetUI<Button>("GoSellButton").interactable = false;
            GetUI<Button>("WarriorUpgradeButton").interactable = false;
            GetUI<Button>("ArcherUpgradeButton").interactable = false;
            GetUI<Button>("BomerUpgradeButton").interactable = false;
        }
        // 설정패널 or 네트워크에러창이 꺼진 경우
        else if (GetUI("ClearPanel").activeSelf == true || GetUI("PausePanel").activeSelf == false || NetworkCheckManager.Instance.IsConnected == true)
        {
            GetUI<Button>("PauseButton").interactable = true;
            GetUI<Button>("SoundButton").interactable = true;
            GetUI<Button>("TImeSpeedButton").interactable = true;
            GetUI<Button>("SpawnButton").interactable = true;
            GetUI<Button>("SpecialSpawnButton").interactable = true;
            GetUI<Button>("GoSellButton").interactable = true;
            GetUI<Button>("WarriorUpgradeButton").interactable = true;
            GetUI<Button>("ArcherUpgradeButton").interactable = true;
            GetUI<Button>("BomerUpgradeButton").interactable = true;
        }
    }

    // To Do: 강화
    // To Do: 몬스터 이미지 표시


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

}
