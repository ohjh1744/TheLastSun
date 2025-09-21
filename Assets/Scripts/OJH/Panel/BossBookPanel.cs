using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BossBookPanel : UIBInder, IAssetLoadable
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }

    [SerializeField] private GameObject _bossBookExplainContext; // 버튼 누를 시 활성화

    [SerializeField] private BossInfoData[] _bossInfoDatas;

    //보스 버튼 LIst에 저장
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

    //보스 초상화 Sprite List에 저장
    private List<Sprite> _bossPortraitSprites = new List<Sprite>();

    private void Awake()
    {
        BindAll();
    }
    void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        _bossBookExplainContext.SetActive(false);
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
        for(int i = 0; i < _bossInfoDatas.Length; i++)
        {
            _bossPortraitButton.Add(GetUI<Button>($"BossPortraitButton{i+1}"));
        }
    }

    private void AddEvent()
    {
        //버튼 이벤트 연결
        GetUI<Button>("BossBookSetFalseButton").onClick.AddListener(SetFalsePanel);
        
        //보스 초상화 눌렀을때 설명 뜨도록
        for(int i = 0; i < _bossPortraitButton.Count; i++)
        {
            int index = i;
            _bossPortraitButton[i].onClick.AddListener(() => ShowBossInfo(index));
        }
    }

    private void LoadAsset()
    {
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgSprite, image, () => { _clearLoadAssetCount++; }); //배경

        //보스 초상화 배경 및 도감 설명 초상화 배경 적용
        AddressableManager.Instance.LoadOnlySprite(_bossPortraitBgSprite, (sprite) => {
            _clearLoadAssetCount++;
            for(int i = 0; i < _bossInfoDatas.Length; i++)
            {
                GetUI<Image>($"BossPortraitBgImage{i + 1}").sprite = sprite;
            }

            //보스 도감 설명 초상화
            GetUI<Image>("BossBookExplainPortraitBgImage").sprite = sprite;
        });

        //보스 초상화 적용
        for(int i = 0; i < _bossInfoDatas.Length; i++)
        {
            int index = i;
            //해당 보스 클리어한 경우
            if (PlayerController.Instance.PlayerData.IsClearStage[index] == true)
            {
                AddressableManager.Instance.LoadOnlySprite(_bossInfoDatas[index].BossUnLockPortraitSprite,  (sprite) => { _clearLoadAssetCount++; _bossPortraitSprites.Add(sprite); GetUI<Image>($"BossPortraitButton{index + 1}").sprite = _bossPortraitSprites[index]; });
            }
            //해당 보스 클리어 못한 경우
            else if (PlayerController.Instance.PlayerData.IsClearStage[index] == false)
            {
                AddressableManager.Instance.LoadOnlySprite(_bossInfoDatas[index].BossLockPortraitSprite, (sprite) => { _clearLoadAssetCount++; _bossPortraitSprites.Add(sprite); GetUI<Image>($"BossPortraitButton{index + 1}").sprite = _bossPortraitSprites[index]; });
            }
        }

        //보스 도감수집현황 Bg
        AddressableManager.Instance.LoadSprite(_bossBookStateSprite, GetUI<Image>("BossBookStateImage"), () => { _clearLoadAssetCount++; });

        //보스 도감 스크롤뷰 Bg, 보스 도감 설명 Bg
        AddressableManager.Instance.LoadOnlySprite(_smallBgSprite, (sprite) => { _clearLoadAssetCount++; GetUI<Image>("BossBookScrollView").sprite = sprite; GetUI<Image>("BossBookExplainBgImage").sprite = sprite; });

        //보스 도감 설명 Text Bg
        AddressableManager.Instance.LoadSprite(_bossBookExplainTextBgSprite, GetUI<Image>("BossBookExplainTextBgImage"), () => { _clearLoadAssetCount++; });

        //보스 도감 X버튼
        AddressableManager.Instance.LoadSprite(_bossBookSetFalseSprite, GetUI<Image>("BossBookSetFalseButton"), () => { _clearLoadAssetCount++; });

        //보스 도감 X버튼 Bg
        AddressableManager.Instance.LoadSprite(_bossBookSetFalseBgSprite, GetUI<Image>("BossBookSetFalseBgImage"), () => { _clearLoadAssetCount++; });


    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }

    private void ShowBossInfo(int bossIndex)
    {
        //설명 내용 set true
        _bossBookExplainContext.SetActive(true);

        //보스 초상화 표시
        Debug.Log(bossIndex);
        GetUI<Image>("BossBookExplainPortraitImage").sprite = _bossPortraitSprites[bossIndex];

        //해당 보스 클리어한 경우
        if (PlayerController.Instance.PlayerData.IsClearStage[bossIndex] == true)
        {
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
            //보스 이름 및 설명 ???표시
            _sb.Clear();
            _sb.Append("???");
            GetUI<TextMeshProUGUI>("BossBookExplainBossNameText").SetText(_sb);
            GetUI<TextMeshProUGUI>("BossBookExplainBossShortInfoText").SetText(_sb);
            GetUI<TextMeshProUGUI>("BossBookExplainBossLongInfoText").SetText(_sb);
        }
    }


}
