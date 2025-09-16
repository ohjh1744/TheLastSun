using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BossBookPanel : UIBInder
{
    [SerializeField] private int _bossCount;

    [SerializeField] private GameObject _bossBookExplainContext; // 버튼 누를 시 활성화

    [SerializeField] private BossInfoData[] _bossInfoDatas;

    private List<Button> _bossPortraitButton = new List<Button>();

    private StringBuilder _sb = new StringBuilder();

    //어드레서블
    [SerializeField] private AssetReferenceSprite _bgSprite;

    [SerializeField] private AssetReferenceSprite _smallBgSprite;

    [SerializeField] private AssetReferenceSprite _bossPortraitBgSprite;

    [SerializeField] private AssetReferenceSprite _bossBookStateSprite;

    [SerializeField] private AssetReferenceSprite _bossBookExplainTextBgSprite;

    [SerializeField] private AssetReferenceSprite _bossBookSetFalseSprite;

    [SerializeField] private AssetReferenceSprite _bossBookSetFalseBgSprite;


    private void Awake()
    {
        BindAll();
    }
    void Start()
    {
        Init();
    }

    private void Init()
    {
        GetUI();
        AddEvent();
        LoadAsset();
    }

    //자주 사용하는 UI 가져오고 저장
    private void GetUI()
    {
        for(int i = 0; i < _bossCount; i++)
        {
            _bossPortraitButton[i] = GetUI<Button>($"BossPortraitBgImage{i+1}");
        }
    }

    private void AddEvent()
    {
        //버튼 이벤트 연결
        GetUI<Button>("BossBookSetFalseButton").onClick.AddListener(SetFalsePanel);
        
        //보스 초상화 눌렀을때 설명 뜨도록
        for(int i = 0; i < _bossPortraitButton.Count; i++)
        {
            _bossPortraitButton[i].onClick.AddListener(() => ShowBossInfo(i));
        }

    }

    private void LoadAsset()
    {
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgSprite, image); //배경

        //보스 초상화 배경 및 초상화 
        for(int i = 0; i < _bossCount; i++)
        {
            AddressableManager.Instance.LoadSprite(_bossPortraitBgSprite, GetUI<Image>($"BossPortraitBgImage{i + 1}"));
            //해당 보스 클리어한 경우
            if (PlayerController.Instance.PlayerData.IsClearStage[i] == true)
            {
                AddressableManager.Instance.LoadSprite(_bossInfoDatas[i].BossUnLockPortraitSprite, GetUI<Image>($"BossPortraitButton{i + 1}"));
            }
            //해당 보스 클리어 못한 경우
            else if (PlayerController.Instance.PlayerData.IsClearStage[i] == false)
            {
                AddressableManager.Instance.LoadSprite(_bossInfoDatas[i].BossLockPortraitSprite, GetUI<Image>($"BossPortraitButton{i + 1}"));
            }
        }

        //보스 도감수집현황 Bg
        AddressableManager.Instance.LoadSprite(_bossBookStateSprite, GetUI<Image>("BossBookStateImage"));

        //보스 도감 스크롤뷰 Bg
        AddressableManager.Instance.LoadSprite(_smallBgSprite, GetUI<Image>("BossBookScrollView"));

        //보스 도감 설명 Bg
        AddressableManager.Instance.LoadSprite(_smallBgSprite, GetUI<Image>("BossBookExplainBgImage"));

        //보스 도감 설명 초상화 Bg
        AddressableManager.Instance.LoadSprite(_bossPortraitBgSprite, GetUI<Image>("BossBookExplainPortraitBgImage"));

        //보스 도감 설명 Text Bg
        AddressableManager.Instance.LoadSprite(_bossBookExplainTextBgSprite, GetUI<Image>("BossBookExplainTextBgImage"));

        //보스 도감 X버튼
        AddressableManager.Instance.LoadSprite(_bossBookSetFalseSprite, GetUI<Image>("BossBookSetFalseButton"));

        //보스 도감 X버튼 Bg
        AddressableManager.Instance.LoadSprite(_bossBookSetFalseBgSprite, GetUI<Image>("BossBookSetFalseBgImage"));

    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }

    private void ShowBossInfo(int bossIndex)
    {
        //버튼 누를시 _bossBookExplainContext활성화 후 보스 초상화, 이름, 설명 표기 
        _bossBookExplainContext.SetActive(true);

        //해당 보스 클리어한 경우
        if (PlayerController.Instance.PlayerData.IsClearStage[bossIndex] == true)
        {
            //보스 해제초상화 표시
            AddressableManager.Instance.LoadSprite(_bossInfoDatas[bossIndex].BossUnLockPortraitSprite, GetUI<Image>("BossBookExplainPortraitImage"));

            //보스 이름 및 설명 표시
            _sb.Clear();
            _sb.Append(_bossInfoDatas[bossIndex].BossName);
            GetUI<TextMeshProUGUI>("BossBookExplainBossNameText").SetText(_sb);

            _sb.Clear();
            _sb.Append(_bossInfoDatas[bossIndex].BossShortInfo);
            GetUI<TextMeshProUGUI>("BossBookExplainBossShortInfoText").SetText(_sb);

            _sb.Clear();
            _sb.Append(_bossInfoDatas[bossIndex].BossLongInfo);
            GetUI<TextMeshProUGUI>("BossBookExplainBossLongInfoText").SetText(_sb);
        }
        //해당 보스 클리어 못한 경우
        else if (PlayerController.Instance.PlayerData.IsClearStage[bossIndex] == false)
        {
            //보스 잠금초상화 표시
            AddressableManager.Instance.LoadSprite(_bossInfoDatas[bossIndex].BossLockPortraitSprite, GetUI<Image>("BossBookExplainPortraitImage"));

            //보스 이름 및 설명 ???표시
            _sb.Clear();
            _sb.Append("???");
            GetUI<TextMeshProUGUI>("BossBookExplainBossNameText").SetText(_sb);
            GetUI<TextMeshProUGUI>("BossBookExplainBossShortInfoText").SetText(_sb);
            GetUI<TextMeshProUGUI>("BossBookExplainBossLongInfoText").SetText(_sb);
        }
    }


}
