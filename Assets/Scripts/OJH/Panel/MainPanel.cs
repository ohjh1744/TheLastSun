using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainPanel : UIBInder, IAssetLoadable
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; }  set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; }  set { _clearLoadAssetCount = value; } }


    //Panel들
    [SerializeField] private GameObject _bossBookPanel;

    [SerializeField] private GameObject _settingPanel;

    //어드레서블
    [SerializeField] private AssetReferenceSprite[] _stageChangeButtonSprites;

    [SerializeField] private AssetReferenceSprite _difficultyLevelSprite;

    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _commonButtonSprite; //랭킹, 보스도감, 설정 버튼에 적용할 이미지

    [SerializeField] private AssetReferenceT<AudioClip> _bgmClip;

    //bgm AudioSource
    [SerializeField] private AudioSource _audio;

    //자주 사용하는 UI
    private List<Button> _stageChangeButtons = new List<Button>(); //0은 left, 1은 right버튼

    private List<Sprite> _stageSprites = new List<Sprite>();

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

    private void Update()
    {
        SetAllButtons();
    }

    private void Init()
    {
        //Bgm 켜주기
        AddressableManager.Instance.LoadSound(_bgmClip, _audio, () => { _clearLoadAssetCount++; });

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
        GetUI<Button>("PlayButton").onClick.AddListener(PlayGame);
        GetUI<Button>("CheckRankButton").onClick.AddListener(SetTrueRankLeaderBoards);
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
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });

        //스테이지 이미지
        //스테이지 Sprite들 List에 저장하고 플레이어가 선택한 스테이지 이미지로 표기
        for(int i = 0; i < _stageDatas.Length; i++)
        {
            AddressableManager.Instance.LoadOnlySprite(_stageDatas[i].StageImageSprite, (sprite) => { _clearLoadAssetCount++; _stageSprites.Add(sprite); });
        }
        GetUI<Image>("StageImage").sprite = _stageSprites[PlayerController.Instance.PlayerData.CurrentStage];
 


        //스테이지 전환 왼쪽, 오른쪽 버튼 이미지 적용
        AddressableManager.Instance.LoadSprite(_stageChangeButtonSprites[0], _stageChangeButtons[0].image, () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_stageChangeButtonSprites[1], _stageChangeButtons[1].image, () => { _clearLoadAssetCount++; });

        //랭킹, 보스도감, 설정 버튼들 이미지 적용
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("RankCheckButton").image, () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("BossBookButton").image, () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_commonButtonSprite, GetUI<Button>("SettingButton").image, () => { _clearLoadAssetCount++; });
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
        if(PlayerController.Instance.PlayerData.CurrentStage == _stageDatas.Length -1)
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
        GetUI<Image>("StageImage").sprite = _stageSprites[PlayerController.Instance.PlayerData.CurrentStage];

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

    private void PlayGame()
    {
        //현재 Player Data저장하고 씬넘기기
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                //전투씬(인게임)으로 넘기기
                SceneManager.LoadScene(2);
                Debug.Log("저장성공 후 게임씬으로 이동");
            }
            else
            {
                //실패시 해야할 일 
                Debug.Log("문제발생으로 저장실패 후 게임씬이동못함");
            }
        });
    }

    private void SetTrueRankLeaderBoards()
    {
        Debug.Log("모든리더보기");
        GpgsManager.Instance.ShowAllLeaderboard();
    }

    private void SetTrueBossBookPanel()
    {
        _bossBookPanel.SetActive(true);
    }

    private void SetTrueSettingPanel()
    {
        _settingPanel.SetActive(true);
    }

    //설정 패널 켜지면 메인 패널 모든 버튼 비활성화
    private void SetAllButtons()
    {
        if (_settingPanel.activeSelf == true)
        {
            GetUI<Button>("CheckRankButton").interactable = false;
            GetUI<Button>("BossBookButton").interactable = false;
            GetUI<Button>("SettingButton").interactable = false;
        }
        else if(_settingPanel.activeSelf == false)
        {
            GetUI<Button>("CheckRankButton").interactable = true;
            GetUI<Button>("BossBookButton").interactable = true;
            GetUI<Button>("SettingButton").interactable = true;
        }
    }


}
