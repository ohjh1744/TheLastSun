using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;



public class MainPanel : UIBInder
{
    // 총 5개, stage1 ==1 , stage2 == 2 ...
    [SerializeField] private int _stageCount;

    //Panel들
    [SerializeField] private GameObject _bossBookPanel;

    [SerializeField] private GameObject _settingPanel;

    //어드레서블
    [SerializeField] private AssetReferenceSprite[] _stageChangeButtonSprites;

    [SerializeField] private AssetReferenceSprite _difficultyLevelSprite;

    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _commonButtonSprite; //랭킹, 보스도감, 설정 버튼에 적용할 이미지

    //자주 사용하는 UI
    private List<Button> _stageChangeButtons = new List<Button>(); //0은 left, 1은 right버튼

    private List<Image> _difficultyLevelImages = new List<Image>();

    //스테이지 Data
    [SerializeField] private StageData[] _stageDatas;

    private StringBuilder _sb;

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        //자주 사용하는 UI가져오고 저장
        GetUI();
        //버튼과 함수 연결
        AddEvent();
        //UI에 어드레서블 에셋 적용
        LoadAsset();
    }

    private void GetUI()
    {
        //자주사용하는 UI불러오고 저장
        _stageChangeButtons.Add(GetUI<Button>("StageChangeLeftButton"));
        _stageChangeButtons.Add(GetUI<Button>("StageChangeRightButton"));

        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel1Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel2Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel2Image_2"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image_2"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image_3"));
    }

    private void AddEvent()
    {
        //버튼과 함수 연결
        _stageChangeButtons[0].onClick.AddListener(ChangeStagePrev);
        _stageChangeButtons[1].onClick.AddListener(ChangeStageNext);
        GetUI<Button>("BossBookButton").onClick.AddListener(SetTrueBossBookPanel);
        GetUI<Button>("SettingButton").onClick.AddListener(SetTrueSettingPanel);

        //이벤트와 함수 연결
        PlayerController.Instance.PlayerData.OnCurrentStageChanged += ChangeStage;
    }

    //UI에 적용할 이미지들 불러오기
    private void LoadAsset()
    {
        //백그라운드 이미지
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image);

        //스테이지 이미지 적용
        ChangeStage();

        //스테이지 전환 왼쪽, 오른쪽 버튼 이미지 적용
        AddressableManager.Instance.LoadSprite(_stageChangeButtonSprites[0], _stageChangeButtons[0].image);
        AddressableManager.Instance.LoadSprite(_stageChangeButtonSprites[1], _stageChangeButtons[1].image);

        //랭킹, 보스도감, 설정 버튼들 이미지 적용
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("RankCheckButton").image);
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("BossBookButton").image);
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("SettingButton").image);
    }

    private void ChangeStagePrev()
    {
        if (PlayerController.Instance.PlayerData.CurrentStage == 0)
        {
            return;
        }
        PlayerController.Instance.PlayerData.CurrentStage--;
    }

    private void ChangeStageNext()
    {
        if(PlayerController.Instance.PlayerData.CurrentStage == _stageCount-1)
        {
            return;
        }
        PlayerController.Instance.PlayerData.CurrentStage++;
    }

    private void ChangeStage()
    {
        //Stage Level
        _sb.Clear();
        _sb.Append(_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageLevel);
        GetUI<TextMeshProUGUI>("StageLevelText").SetText(_sb);

        //Name Text 변경
        _sb.Clear();
        _sb.Append(_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageName);
        GetUI<TextMeshProUGUI>("StageNameText").SetText(_sb);

        //Stage Image 변경
        AddressableManager.Instance.LoadSprite(_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageImageSprite, GetUI<Image>("StageImage"));

        //Stage difficulty 변경
        if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 1)
        {
            GetUI("DifficultyLevel1Images").SetActive(true);
            GetUI("DifficultyLevel2Images").SetActive(false);
            GetUI("DifficultyLevel3Images").SetActive(false);
        }
        else if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 2)
        {
            GetUI("DifficultyLevel1Images").SetActive(false);
            GetUI("DifficultyLevel2Images").SetActive(true);
            GetUI("DifficultyLevel3Images").SetActive(false);
        }
        else if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 3)
        {
            GetUI("DifficultyLevel1Images").SetActive(false);
            GetUI("DifficultyLevel2Images").SetActive(false);
            GetUI("DifficultyLevel3Images").SetActive(true);
        }
    }

    private void SetTrueBossBookPanel()
    {
        _bossBookPanel.SetActive(true);
    }

    private void SetTrueSettingPanel()
    {
        _settingPanel.SetActive(true);
    }




}
