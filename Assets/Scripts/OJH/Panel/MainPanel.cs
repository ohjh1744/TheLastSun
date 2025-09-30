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



public class MainPanel : UIBInder, IAssetLoadable
{
    private enum EMainButton { StageChangeLeftButton, StageChangeRightButton, PlayButton, CheckRankButton, BossBookButton, SettingButton , Length};

    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; }  set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; }  set { _clearLoadAssetCount = value; } }
    #endregion

    #region On Off Objects
    //Panel들
    [SerializeField] private GameObject _bossBookPanel;

    [SerializeField] private GameObject _settingPanel;

    //난이도 관련 이미지 Object
    private GameObject _currentDifficultyLevelImage;
    #endregion

    #region Addressable Assets
    //어드레서블
    [SerializeField] private AssetReferenceSprite _stageChangeLeftButtonSprite;

    [SerializeField] private AssetReferenceSprite _stageChangeRightButtonSprite;

    [SerializeField] private AssetReferenceSprite _LockSprite;

    [SerializeField] private AssetReferenceSprite _difficultyLevelSprite;

    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _commonButtonSprite; //랭킹, 보스도감, 설정 버튼에 적용할 이미지

    [SerializeField] private AssetReferenceT<AudioClip> _bgmClip;
    #endregion

    #region 저장하여 관리하는 UIs
    //한꺼번에 관리할때 편리한 UI관련 변수
    private List<Sprite> _stageSprites = new List<Sprite>();

    private List<Button> _buttons = new List<Button>();

    [SerializeField] private List<Image> _difficultyLevelImages;
    #endregion

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
        LoadAsset();
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
        PlayerController.Instance.PlayerData.OnCurrentStageChanged += ChangeStageDetail;
    }

    //UI에 적용할 이미지들 불러오기
    private void LoadAsset()
    {
        //Bgm 세팅
        AddressableManager.Instance.LoadSound(_bgmClip, _audio, () => { _clearLoadAssetCount++; SetSound(); });

        //백그라운드 이미지
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });

        //LockImage
        AddressableManager.Instance.LoadSprite(_LockSprite, GetUI<Image>("LockImage"), () => { _clearLoadAssetCount++; });

        //스테이지 이미지
        //스테이지 Sprite들 List에 저장하고 플레이어가 선택한 스테이지 이미지로 표기
        for (int i = 0; i < _stageDatas.Length; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_stageDatas[i].StageImageSprite, (sprite) => { 
                _clearLoadAssetCount++; 
                _stageSprites.Add(sprite);
                if (PlayerController.Instance.PlayerData.CurrentStage == index)
                {
                    ChangeStageLevelNameImage(sprite);
                }
            });
        }


        //스테이지 전환 왼쪽, 오른쪽 버튼 이미지 적용
        AddressableManager.Instance.LoadSprite(_stageChangeLeftButtonSprite, GetUI<Button>("StageChangeLeftButton").image, () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_stageChangeRightButtonSprite, GetUI<Button>("StageChangeRightButton").image, () => { _clearLoadAssetCount++; });

        //난이도 이미지 적용 
        AddressableManager.Instance.LoadOnlySprite(_difficultyLevelSprite, (sprite) => {
            _clearLoadAssetCount++;
            for(int i = 0; i < _difficultyLevelImages.Count; i++)
            {
                _difficultyLevelImages[i].sprite = sprite;
            }

            //난이도 표시
            ChangeStageDifficulty(GetUI($"DifficultyLevel{_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty}Images"));

        });

        //플레이, 랭킹, 보스도감, 설정 버튼들 이미지 적용
        AddressableManager.Instance.LoadOnlySprite(_commonButtonSprite,(sprite) => { 
            _clearLoadAssetCount++;
            GetUI<Image>("PlayButton").sprite = sprite;
            GetUI<Image>("CheckRankButton").sprite = sprite;
            GetUI<Image>("BossBookButton").sprite = sprite;
            GetUI<Image>("SettingButton").sprite = sprite; 
        });
    }

    // 스테이지 변경
    private void ChangeStageNum(bool isNext)
    {
        if (PlayerController.Instance.PlayerData.CurrentStage > 0 && isNext == false)
        {
            PlayerController.Instance.PlayerData.CurrentStage--;
        }
        else if (PlayerController.Instance.PlayerData.CurrentStage <_stageDatas.Length - 1 && isNext == true)
        {
            PlayerController.Instance.PlayerData.CurrentStage++;
        }
    }

    //스테이지 변경에 따른 수정
    private void ChangeStageDetail()
    {
        ChangeStageLevelNameImage(_stageSprites[PlayerController.Instance.PlayerData.CurrentStage]);
        ChangeStageDifficulty(GetUI($"DifficultyLevel{_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty}Images"));
    }

    //Stage Level, Name , Sprite 변경
    private void ChangeStageLevelNameImage (Sprite stageSprite)
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
        GetUI<Image>("StageImage").sprite = stageSprite;

        //첫번째 스테이지와 이전스테이지클리어시 해당스테이지는 Lock해제
        if (PlayerController.Instance.PlayerData.CurrentStage == 0  || PlayerController.Instance.PlayerData.IsClearStage[PlayerController.Instance.PlayerData.CurrentStage - 1] == true)
        {
            GetUI<Image>("StageImage").color = _stageDatas[PlayerController.Instance.PlayerData.CurrentStage].UnLockStageColor;
            GetUI<Image>("LockImage").gameObject.SetActive(false);
        }
        else
        {
            GetUI<Image>("StageImage").color = _stageDatas[PlayerController.Instance.PlayerData.CurrentStage].LockStageColor;
            GetUI<Image>("LockImage").gameObject.SetActive(true);
        }
    }

    //Stage difficulty 변경
    private void ChangeStageDifficulty(GameObject newDifficultyLevel)
    {
        if(_currentDifficultyLevelImage != null)
        {
            _currentDifficultyLevelImage.SetActive(false);
        }
        _currentDifficultyLevelImage = newDifficultyLevel;
        _currentDifficultyLevelImage.SetActive(true);
    }

    private void PlayGame()
    {
        //현재 Player Data저장하고 씬넘기기
        // 앞서 1차방어막으로 네트워크팝업창이 뜨긴하겠으나 한번더 안전하게 네트워크 체크
        if (NetworkCheckManager.Instance.IsConnected == true)
        {
            GpgsManager.Instance.SaveData((status) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    //이벤트와 함수 해제
                    PlayerController.Instance.PlayerData.OnCurrentStageChanged -= ChangeStageDetail;

                    //전투씬(인게임)으로 넘기기
                    SceneManager.LoadScene(2);
                    Debug.Log("저장성공 후 게임씬으로 이동");
                }
                else
                {
                    Debug.Log("문제발생으로 저장실패 후 게임씬이동못함");
                }
            });
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

    private void SetSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == false)
        {
            _audio.Stop();
        }
        else if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            _audio.Play();
        }
    }
}
