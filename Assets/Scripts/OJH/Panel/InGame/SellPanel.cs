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
        //¿µ¿õ À¯´ÖÀÌ 0ÀÌ ¾Æ´Ñ °æ¿ì¸¸ 
        if(ObjectPoolManager.Instance.GetHeroNum(heroIndex) != 0)
        {
            //»ç¿îµå
            _sfx.PlayOneShot(_sellClip);
            //¿µ¿õ Á¦°Å
            ObjectPoolManager.Instance.RemoveObject(ObjectPoolManager.Instance.HeroPools, heroIndex);

            //¿µ¿õ ¼ö ÁÙ¿©ÁÖ±â
            ObjectPoolManager.Instance.SetHeroNum(heroIndex, ObjectPoolManager.Instance.GetHeroNum(heroIndex) - 1);

            //Áª È¹µæ
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



}
