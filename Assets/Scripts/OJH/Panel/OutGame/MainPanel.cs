using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;



public class MainPanel : UIBInder
{
    private enum EMainButton { StageChangeLeftButton, StageChangeRightButton, PlayButton, CheckRankButton, BossBookButton, SettingButton , Length};


    //Panel들
    [SerializeField] private GameObject _bossBookPanel;
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _readyForGamePanel;

    //난이도 관련 이미지 Object
    private GameObject _currentDifficultyLevelImage;


    //한꺼번에 관리할때 편리한 UI관련 변수
    [SerializeField] private List<Image> _difficultyLevelImages;
    private List<Button> _buttons = new List<Button>();
    private List<Image> _stageImages = new List<Image>();

    //오디오
    [SerializeField] private AudioSource _audio;

    //스테이지Data
    [SerializeField] private StageData[] _stageDatas;

    private StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        Debug.Log($"PlayerData: {JsonUtility.ToJson(PlayerController.Instance.PlayerData)}");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Init();
    }

    private void Update()
    {
        SetAllButtons();
    }

    private void Init()
    {
        GetUI();
        AddEvent();
        ChangeStageDetail(PlayerController.Instance.PlayerData.CurrentStage);
    }

    private void GetUI()
    {
        //자주사용하는 UI불러오고 저장
        for(int i = 0; i < _difficultyLevelImages.Count; i++)
        {
            _difficultyLevelImages[i] = GetUI<Image>($"DifficultyLevelImage{i+1}");
        }
        for(int i = 0; i < (int)EMainButton.Length; i++)
        {
            _buttons.Add(GetUI<Button>($"{((EMainButton)i).ToString()}"));
        }
        for (int i = 0; i < _stageDatas.Length; i++)
        {
            _stageImages.Add(GetUI<Image>($"Stage{i+1}Image"));
        }
    }

    private void AddEvent()
    {
        //버튼과 함수 연결
        _buttons[(int)EMainButton.StageChangeLeftButton].onClick.AddListener(() => ChangeStageNum(false));
        _buttons[(int)EMainButton.StageChangeRightButton].onClick.AddListener(() => ChangeStageNum(true));
        _buttons[(int)EMainButton.PlayButton].onClick.AddListener(PlayGame);
        _buttons[(int)EMainButton.CheckRankButton].onClick.AddListener(SetTrueRankLeaderBoards);
        _buttons[(int)EMainButton.BossBookButton].onClick.AddListener(() => SetTruePanel(_bossBookPanel));
        _buttons[(int)EMainButton.SettingButton].onClick.AddListener(() => SetTruePanel(_settingPanel));

        //이벤트와 함수 연결
        //PlayerController.Instance.PlayerData.OnCurrentStageChanged += ChangeStageDetail;
    }



    // 스테이지 변경
    private void ChangeStageNum(bool isNext)
    {
        if (PlayerController.Instance.PlayerData.CurrentStage > 0 && isNext == false)
        {
            PlayerController.Instance.PlayerData.CurrentStage--;
            Debug.Log($"_isChoiceStage 변경 {PlayerController.Instance.PlayerData.CurrentStage}");
            ChangeStageDetail(PlayerController.Instance.PlayerData.CurrentStage);
        }
        else if (PlayerController.Instance.PlayerData.CurrentStage < _stageDatas.Length - 1 && isNext == true)
        {
            PlayerController.Instance.PlayerData.CurrentStage++;
            Debug.Log($"_isChoiceStage 변경 {PlayerController.Instance.PlayerData.CurrentStage}");
            ChangeStageDetail(PlayerController.Instance.PlayerData.CurrentStage);
        }
    }

    //스테이지 변경에 따른 수정
    private void ChangeStageDetail(int playerChoiceStage)
    {
        ChangeStageLevelNameImage(playerChoiceStage);
        ChangeImage(playerChoiceStage, GetUI($"DifficultyLevel{_stageDatas[playerChoiceStage].StageDifficulty}Images"));
    }

    //Stage Level, Name , Sprite 변경
    private void ChangeStageLevelNameImage (int playerChoiceStage)
    {
        //Stage Level
        _sb.Clear();
        _sb.Append(_stageDatas[playerChoiceStage].StageLevel);
        GetUI<TextMeshProUGUI>("StageLevelText").SetText(_sb);


        //Name Text 변경
        _sb.Clear();
        _sb.Append(_stageDatas[playerChoiceStage].StageName);
        GetUI<TextMeshProUGUI>("StageNameText").SetText(_sb);
    }

    private void ChangeImage(int playerChoiceStage, GameObject newDifficultyLevel)
    {
        //Stage Image 변경
        for (int i = 0; i < _stageImages.Count; i++)
        {
            if (i == playerChoiceStage)
            {
                _stageImages[i].gameObject.SetActive(true);
            }
            else
            {
                _stageImages[i].gameObject.SetActive(false);
            }
        }

        //첫번째 스테이지와 이전스테이지클리어시 해당스테이지는 Lock해제
        if (playerChoiceStage == 0 || PlayerController.Instance.PlayerData.IsClearStage[playerChoiceStage - 1] == true)
        {
            _stageImages[playerChoiceStage].color = _stageDatas[playerChoiceStage].UnLockStageColor;
            GetUI<Image>("LockImage").gameObject.SetActive(false);
        }
        else
        {
            _stageImages[playerChoiceStage].color = _stageDatas[playerChoiceStage].LockStageColor;
            GetUI<Image>("LockImage").gameObject.SetActive(true);
        }

        //난이도 이미지 변경
        if (_currentDifficultyLevelImage != null)
        {
            _currentDifficultyLevelImage.SetActive(false);
        }
        _currentDifficultyLevelImage = newDifficultyLevel;
        _currentDifficultyLevelImage.SetActive(true);

        Debug.Log("해골 변경");
    }


    private void PlayGame()
    {
        //1번째 스테이지나 해금된 스테이지만 할수있도록 
        if (PlayerController.Instance.PlayerData.CurrentStage == 0 || PlayerController.Instance.PlayerData.IsClearStage[PlayerController.Instance.PlayerData.CurrentStage - 1] == true)
        {
            //안전하게 네트워크 체크한번더
            if(NetworkCheckManager.Instance.IsConnected == true)
            {
                _readyForGamePanel.SetActive(true);
                //데이터 저장후 씬 넘기기
                GpgsManager.Instance.SaveData((success) =>
                {
                    if (success == SavedGameRequestStatus.Success)
                    {
                        Debug.Log("게임씬으로 이동");
                        SceneManager.LoadScene(2);
                    }
                    else
                    {
                        _readyForGamePanel.SetActive(false);
                    }
                });
            }
        }
    }

    private void SetTrueRankLeaderBoards()
    {
        Debug.Log("모든리더보기");
        GpgsManager.Instance.ShowAllLeaderboard();
    }

    private void SetTruePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    private void SetAllButtons()
    {
        //설정패널 or 네트워크에러패널이 뜨는 경우
        if (_settingPanel.activeSelf == true || NetworkCheckManager.Instance.IsConnected == false)
        {
            for(int i = 0; i < (int)EMainButton.Length; i++)
            {
                _buttons[i].interactable = false;
            }
        }
        // 설정패널 or 네트워크에러창이 꺼진 경우
        else if(_settingPanel.activeSelf == false || NetworkCheckManager.Instance.IsConnected == true)
        {
            for (int i = 0; i < (int)EMainButton.Length; i++)
            {
                _buttons[i].interactable = true;
            }
        }
    }

}
