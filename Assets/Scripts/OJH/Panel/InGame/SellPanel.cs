using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SellPanel : UIBInder
{
    private enum EHeroType{Warrior, Archer, Bomer};

    [SerializeField] private AudioSource _sfx;

    private AudioClip _sellClip;

    StringBuilder _sb = new StringBuilder();

    private List<TextMeshProUGUI> _heroNumSellTexts = new List<TextMeshProUGUI>();
    void Awake()
    {
        BindAll();
        Init();
    }

    private void OnEnable()
    {
        ChangeHeroType(EHeroType.Warrior);
        for(int i = 0; i <= (int)EHeroPool.E_Bomer; i++)
        {
            ShowHeroRemainNum(i);
        }
    }
    void Start()
    {
        AddEvent();
        ShowJemNum();
    }

    private void Update()
    {
        SetAllButtons();
    }


    private void Init()
    {
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("NormalWarriorNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("RareWarriorNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("AncientWarriorNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("LegendWarriorNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("EpicWarriorNumSellText"));

        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("NormalArcherNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("RareArcherNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("AncientArcherNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("LegendArcherNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("EpicArcherNumSellText"));

        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("NormalBomerNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("RareBomerNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("AncientBomerNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("LegendBomerNumSellText"));
        _heroNumSellTexts.Add(GetUI<TextMeshProUGUI>("EpicBomerNumSellText"));

        _sellClip = _sfx.clip;
    }


    private void AddEvent()
    {
        GetUI<Button>("SellPanelSetFalseButton").onClick.AddListener(SetFalsePanel);
        GetUI<Button>("ChangeSellWarriorButton").onClick.AddListener(() => ChangeHeroType(EHeroType.Warrior));
        GetUI<Button>("ChangeSellArcherButton").onClick.AddListener(() => ChangeHeroType(EHeroType.Archer));
        GetUI<Button>("ChangeSellBomerButton").onClick.AddListener(() => ChangeHeroType(EHeroType.Bomer));

        GetUI<Button>("NormalWarriorSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.N_Warrior));
        GetUI<Button>("NormalArcherSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.N_Archer));
        GetUI<Button>("NormalBomerSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.N_Bomer));

        GetUI<Button>("RareWarriorSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.R_Warrior));
        GetUI<Button>("RareArcherSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.R_Archer));
        GetUI<Button>("RareBomerSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.R_Bomer));

        GetUI<Button>("AncientWarriorSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.A_Warrior));
        GetUI<Button>("AncientArcherSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.A_Archer));
        GetUI<Button>("AncientBomerSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.A_Bomer));

        GetUI<Button>("LegendWarriorSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.L_Warrior));
        GetUI<Button>("LegendArcherSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.L_Archer));
        GetUI<Button>("LegendBomerSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.L_Bomer));

        GetUI<Button>("EpicWarriorSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.E_Warrior));
        GetUI<Button>("EpicArcherSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.E_Archer));
        GetUI<Button>("EpicBomerSellButton").onClick.AddListener(() => SellHero((int)EHeroPool.E_Bomer));

        ObjectPoolManager.Instance.HeroNumOnChanged += ShowHeroRemainNum;
    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }
    private void ChangeHeroType(EHeroType eheroType)
    {
        switch (eheroType)
        {
            case EHeroType.Warrior:
                for(int i = 0; i <= (int)EHeroGrade.Epic; i++)
                {
                    GetUI($"{((EHeroGrade)i).ToString()}WarriorSellButton").SetActive(true);
                    GetUI($"{((EHeroGrade)i).ToString()}ArcherSellButton").SetActive(false);
                    GetUI($"{((EHeroGrade)i).ToString()}BomerSellButton").SetActive(false);
                }
                break;
            case EHeroType.Archer:
                for (int i = 0; i <= (int)EHeroGrade.Epic; i++)
                {
                    GetUI($"{((EHeroGrade)i).ToString()}ArcherSellButton").SetActive(true);
                    GetUI($"{((EHeroGrade)i).ToString()}WarriorSellButton").SetActive(false);
                    GetUI($"{((EHeroGrade)i).ToString()}BomerSellButton").SetActive(false);
                }
                break;
            case EHeroType.Bomer:
                for (int i = 0; i <= (int)EHeroGrade.Epic; i++)
                {
                    GetUI($"{((EHeroGrade)i).ToString()}BomerSellButton").SetActive(true);
                    GetUI($"{((EHeroGrade)i).ToString()}WarriorSellButton").SetActive(false);
                    GetUI($"{((EHeroGrade)i).ToString()}ArcherSellButton").SetActive(false);
                }
                break;
        }
    }

    private void ShowJemNum()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= (int)EHeroGrade.Epic; i++)
        {
            sb.Clear();
            sb.Append(InGameManager.Instance.JemNumsForSell[i]);
            GetUI<TextMeshProUGUI>($"Get{((EHeroGrade)i).ToString()}JemCountText").SetText(sb);
        }
    }

    private void SellHero(int heroIndex)
    {
        //영웅 유닛이 0이 아닌 경우만 
        if(ObjectPoolManager.Instance.GetHeroNum(heroIndex) != 0)
        {
            //사운드
            if(PlayerController.Instance.PlayerData.IsSound == true)
            {
                _sfx.PlayOneShot(_sellClip);
            }

            //영웅 제거
            ObjectPoolManager.Instance.RemoveObject(ObjectPoolManager.Instance.HeroPools, heroIndex);

            //영웅 수 줄여주기
            ObjectPoolManager.Instance.SetHeroNum(heroIndex, ObjectPoolManager.Instance.GetHeroNum(heroIndex) - 1);

            //젬 획득
            if(heroIndex == (int)EHeroPool.N_Warrior || heroIndex == (int)EHeroPool.N_Archer || heroIndex == (int)EHeroPool.N_Bomer)
            {
                InGameManager.Instance.JemNum += InGameManager.Instance.JemNumsForSell[(int)EHeroGrade.Normal];
            }
            else if (heroIndex == (int)EHeroPool.R_Warrior || heroIndex == (int)EHeroPool.R_Archer || heroIndex == (int)EHeroPool.R_Bomer)
            {
                InGameManager.Instance.JemNum += InGameManager.Instance.JemNumsForSell[(int)EHeroGrade.Rare];
            }
            else if (heroIndex == (int)EHeroPool.A_Warrior || heroIndex == (int)EHeroPool.A_Archer || heroIndex == (int)EHeroPool.A_Bomer)
            {
                InGameManager.Instance.JemNum += InGameManager.Instance.JemNumsForSell[(int)EHeroGrade.Ancient];
            }
            else if (heroIndex == (int)EHeroPool.L_Warrior || heroIndex == (int)EHeroPool.L_Archer || heroIndex == (int)EHeroPool.L_Bomer)
            {
                InGameManager.Instance.JemNum += InGameManager.Instance.JemNumsForSell[(int)EHeroGrade.Legend];
            }
            else if (heroIndex == (int)EHeroPool.E_Warrior || heroIndex == (int)EHeroPool.E_Archer || heroIndex == (int)EHeroPool.E_Bomer)
            {
                InGameManager.Instance.JemNum += InGameManager.Instance.JemNumsForSell[(int)EHeroGrade.Epic];
            }
        }
 
    }

    private void ShowHeroRemainNum(int heroIndex)
    {
        _sb.Clear();
        _sb.Append(ObjectPoolManager.Instance.GetHeroNum(heroIndex));
        _heroNumSellTexts[heroIndex].SetText(_sb);
    }

    private void SetAllButtons()
    {
        //설정패널 or 네트워크에러패널이 뜨는 경우
        if (InGameManager.Instance.GameState == EGameState.Win || InGameManager.Instance.GameState == EGameState.Defeat || InGameManager.Instance.GameState == EGameState.Pause || NetworkCheckManager.Instance.IsConnected == false)
        {
            GetUI<Button>("SellPanelSetFalseButton").interactable = false;
            GetUI<Button>("ChangeSellWarriorButton").interactable = false;
            GetUI<Button>("ChangeSellArcherButton").interactable = false;
            GetUI<Button>("ChangeSellBomerButton").interactable = false;

            GetUI<Button>("NormalWarriorSellButton").interactable = false;
            GetUI<Button>("NormalArcherSellButton").interactable = false;
            GetUI<Button>("NormalBomerSellButton").interactable = false;

            GetUI<Button>("RareWarriorSellButton").interactable = false;
            GetUI<Button>("RareArcherSellButton").interactable = false;
            GetUI<Button>("RareBomerSellButton").interactable = false;

            GetUI<Button>("AncientWarriorSellButton").interactable = false;
            GetUI<Button>("AncientArcherSellButton").interactable = false;
            GetUI<Button>("AncientBomerSellButton").interactable = false;

            GetUI<Button>("LegendWarriorSellButton").interactable = false;
            GetUI<Button>("LegendArcherSellButton").interactable = false;   
            GetUI<Button>("LegendBomerSellButton").interactable = false;

            GetUI<Button>("EpicWarriorSellButton").interactable = false;
            GetUI<Button>("EpicArcherSellButton").interactable = false;
            GetUI<Button>("EpicBomerSellButton").interactable = false;
        }
        // 설정패널 or 네트워크에러창이 꺼진 경우
        else if (InGameManager.Instance.GameState == EGameState.Play || NetworkCheckManager.Instance.IsConnected == true)
        {
            GetUI<Button>("SellPanelSetFalseButton").interactable = true;
            GetUI<Button>("ChangeSellWarriorButton").interactable = true;
            GetUI<Button>("ChangeSellArcherButton").interactable = true;
            GetUI<Button>("ChangeSellBomerButton").interactable = true;

            GetUI<Button>("NormalWarriorSellButton").interactable = true;
            GetUI<Button>("NormalArcherSellButton").interactable = true;
            GetUI<Button>("NormalBomerSellButton").interactable = true;

            GetUI<Button>("RareWarriorSellButton").interactable = true;
            GetUI<Button>("RareArcherSellButton").interactable = true;
            GetUI<Button>("RareBomerSellButton").interactable = true;

            GetUI<Button>("AncientWarriorSellButton").interactable = true;
            GetUI<Button>("AncientArcherSellButton").interactable = true;
            GetUI<Button>("AncientBomerSellButton").interactable = true;

            GetUI<Button>("LegendWarriorSellButton").interactable = true;
            GetUI<Button>("LegendArcherSellButton").interactable = true;
            GetUI<Button>("LegendBomerSellButton").interactable = true;

            GetUI<Button>("EpicWarriorSellButton").interactable = true;
            GetUI<Button>("EpicArcherSellButton").interactable = true;
            GetUI<Button>("EpicBomerSellButton").interactable = true;
        }
    }



}
