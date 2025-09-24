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
    [SerializeField] private AssetReferenceSprite _stageChangeLeftButtonSprite;

    [SerializeField] private AssetReferenceSprite _stageChangeRightButtonSprite;

    [SerializeField] private AssetReferenceSprite _difficultyLevelSprite;

    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _commonButtonSprite; //랭킹, 보스도감, 설정 버튼에 적용할 이미지

    [SerializeField] private AssetReferenceT<AudioClip> _bgmClip;

    private List<Image> _difficultyLevelImages = new List<Image>();

    private List<Sprite> _stageSprites = new List<Sprite>();

    //bgm AudioSource
    [SerializeField] private AudioSource _audio;

    //스테이지 Data
    [SerializeField] private StageData[] _stageDatas;

    private StringBuilder _sb = new StringBuilder();

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
        GetUI<Button>("StageChangeLeftButton").onClick.AddListener(ChangeStagePrev);
        GetUI<Button>("StageChangeRightButton").onClick.AddListener(ChangeStageNext);
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
        //Bgm 세팅
        AddressableManager.Instance.LoadSound(_bgmClip, _audio, () => { _clearLoadAssetCount++; SetSound(); });

        //백그라운드 이미지
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });

        //스테이지 이미지
        //스테이지 Sprite들 List에 저장하고 플레이어가 선택한 스테이지 이미지로 표기
        for(int i = 0; i < _stageDatas.Length; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_stageDatas[i].StageImageSprite, (sprite) => { 
                _clearLoadAssetCount++; 
                _stageSprites.Add(sprite);
                if (PlayerController.Instance.PlayerData.CurrentStage == index)
                {
                    //Stage Level 표시
                    _sb.Clear();
                    _sb.Append(_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageLevel);
                    GetUI<TextMeshProUGUI>("StageLevelText").SetText(_sb);

                    //Stage name 표시
                    _sb.Clear();
                    _sb.Append(_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageName);
                    GetUI<TextMeshProUGUI>("StageNameText").SetText(_sb);

                    //이미지 설정
                    GetUI<Image>("StageImage").sprite = sprite;
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

            //이미지 가지고온뒤 CUrrentStage에 따른 해골 난이도 이미지 표시
            if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 1)
            {
                GetUI("DifficultyLevel1Images").SetActive(true);
            }
            else if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 2)
            {
                GetUI("DifficultyLevel2Images").SetActive(true);
            }
            else if (_stageDatas[PlayerController.Instance.PlayerData.CurrentStage].StageDifficulty == 3)
            {
                GetUI("DifficultyLevel3Images").SetActive(true);
            }

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
        // 앞서 1차방어막으로 네트워크팝업창이 뜨긴하겠으나 한번더 안전하게 네트워크 체크
        if (NetworkCheckManager.Instance.IsConnected == true)
        {
            GpgsManager.Instance.SaveData((status) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    //이벤트와 함수 해제
                    PlayerController.Instance.PlayerData.OnCurrentStageChanged -= ChangeStage;

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
        //설정패널 or 네트워크에러패널이 뜨는 경우
        if (_settingPanel.activeSelf == true || NetworkCheckManager.Instance.IsConnected == false)
        {
            GetUI<Button>("PlayButton").interactable = false;
            GetUI<Button>("CheckRankButton").interactable = false;
            GetUI<Button>("BossBookButton").interactable = false;
            GetUI<Button>("SettingButton").interactable = false;
            GetUI<Button>("StageChangeLeftButton").interactable = false;
            GetUI<Button>("StageChangeRightButton").interactable = false;
        }
        // 설정패널 or 네트워크에러창이 꺼진 경우
        else if(_settingPanel.activeSelf == false || NetworkCheckManager.Instance.IsConnected == true)
        {
            GetUI<Button>("PlayButton").interactable = true;
            GetUI<Button>("CheckRankButton").interactable = true;
            GetUI<Button>("BossBookButton").interactable = true;
            GetUI<Button>("SettingButton").interactable = true;
            GetUI<Button>("StageChangeLeftButton").interactable = true;
            GetUI<Button>("StageChangeRightButton").interactable = true;
        }
    }

    private void SetSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == false)
        {
            //사운드 끄기
            _audio.Stop();
        }
        else if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            //사운드 켜기
            _audio.Play();
        }
    }


}
