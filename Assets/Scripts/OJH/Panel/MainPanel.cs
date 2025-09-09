using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


public enum EStage {First, Second, Third, Fourth, Fifth}

public class MainPanel : UIBInder
{
    // 총 5개, stage1 ==1 , stage2 == 2 ...
    [SerializeField] private int _stageCount;

    //어드레서블 
    [SerializeField] private AssetReferenceSprite[] _stageImageSprites;

    [SerializeField] private AssetReferenceSprite[] _stageChangeButtonSprites;

    [SerializeField] private AssetReferenceSprite _difficultyLevelSprite;

    //Sprite 적용할 에셋들
    private List<Button> _stageChangeButtons = new List<Button>();

    private Image _stageImage;

    private List<Image> _difficultyLevelImages = new List<Image>();

    private Button _rankCheckButton;

    private Button _bossBookButton;

    private Button _settingButton;

    //Text들
    private TextMeshProUGUI _stageLevelText;

    private TextMeshProUGUI _stageNameText;

    private static string[] _stageLevelDetails = { "The First Sun", "The Second Sun", "The Third Sun", "The Fourth Sun", "The Last Sun"};

    private static string[] _stageNameDetails = { "첫 번째 영역", "두 번째 영역", "세 번째 영역", "네 번째 영역", "다섯 번째 영역" };

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
        //자주사용하는 UI불러오고 저장

        _stageLevelText = GetUI<TextMeshProUGUI>("StageLevelText");
        _stageNameText = GetUI<TextMeshProUGUI>("StageNameText");

        _stageChangeButtons.Add(GetUI<Button>("StageChangeLeftButton"));
        _stageChangeButtons.Add(GetUI<Button>("StageChangeRightButton"));

        _stageImage = GetUI<Image>("StageImage");

        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel1Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel2Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel2Image_2"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image_2"));
        _difficultyLevelImages.Add(GetUI<Image>("DifficultyLevel3Image_3"));

        _rankCheckButton = GetUI<Button>("RankCheckButton");
        _bossBookButton = GetUI<Button>("BossBookButton");
        _settingButton = GetUI<Button>("SettingButton");

        //버튼과 함수 연결
        _stageChangeButtons[0].onClick.AddListener(ChangeStagePrev);
        _stageChangeButtons[1].onClick.AddListener(ChangeStageNext);

        //이벤트와 함수 연결
        PlayerController.Instance.PlayerData.OnCurrentStageChanged += ChangeStage;

    }

    private void ChangeStagePrev()
    {
        if (PlayerController.Instance.PlayerData.CurrentStage == 1)
        {
            return;
        }
        PlayerController.Instance.PlayerData.CurrentStage--;
    }

    private void ChangeStageNext()
    {
        if(PlayerController.Instance.PlayerData.CurrentStage == _stageCount)
        {
            return;
        }
        PlayerController.Instance.PlayerData.CurrentStage++;
    }

    private void ChangeStage()
    {
        //Stage Level
        _sb.Clear();
        _sb.Append(_stageLevelDetails[PlayerController.Instance.PlayerData.CurrentStage]);
        _stageLevelText.SetText(_sb);

        //Name Text 변경
        _sb.Clear();
        _sb.Append(_stageNameDetails[PlayerController.Instance.PlayerData.CurrentStage]);
        _stageNameText.SetText(_sb);

        //Stage Image 변경
        AddressableManager.Instance.LoadSprite(_stageImageSprites[PlayerController.Instance.PlayerData.CurrentStage], _stageImage);

        //Stage difficulty 변경
        if (PlayerController.Instance.PlayerData.CurrentStage == 1)
        {
            GetUI("DifficultyLevel1Images").SetActive(true);
            GetUI("DifficultyLevel2Images").SetActive(false);
            GetUI("DifficultyLevel3Images").SetActive(false);
        }
        else if (PlayerController.Instance.PlayerData.CurrentStage == 2 || PlayerController.Instance.PlayerData.CurrentStage == 3)
        {
            GetUI("DifficultyLevel1Images").SetActive(false);
            GetUI("DifficultyLevel2Images").SetActive(true);
            GetUI("DifficultyLevel3Images").SetActive(false);
        }
        else if (PlayerController.Instance.PlayerData.CurrentStage == 4 || PlayerController.Instance.PlayerData.CurrentStage == 5)
        {
            GetUI("DifficultyLevel1Images").SetActive(false);
            GetUI("DifficultyLevel2Images").SetActive(false);
            GetUI("DifficultyLevel3Images").SetActive(true);
        }

    }




}
