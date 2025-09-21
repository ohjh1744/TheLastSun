using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using GooglePlayGames.BasicApi.SavedGame;
public class SettingPanel : UIBInder, IAssetLoadable
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
     private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }

    //bgm AudioSource
    [SerializeField] private AudioSource _audio;

    [SerializeField] private GameObject _CreditPanel;

    StringBuilder _sb  = new StringBuilder();

    //어드레서블
    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _setFalseBgSprite;

    [SerializeField] private AssetReferenceSprite _setFalseSprite;

    [SerializeField] private AssetReferenceSprite _buttonSprite;

    [SerializeField] private AssetReferenceSprite _settingNameBgSprite;

    private string _packageName;

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

        _packageName = Application.identifier;
        
        //사운드 Text 초기화
        if(PlayerController.Instance.PlayerData.IsSound == true)
        {
            _sb.Clear();
            _sb.Append("사운드ON");
            GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
        }
        else
        {
            _sb.Clear();
            _sb.Append("사운드OFF");
            GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
        }

        AddEvent();
        LoadAsset();
    }
    private void AddEvent()
    {
        GetUI<Button>("SettingSetFalseButton").onClick.AddListener(SetFalsePanel);
        GetUI<Button>("SetMusicButton").onClick.AddListener(SetSound);
        GetUI<Button>("ShowCreditButton").onClick.AddListener(SetTrueCredit);
        GetUI<Button>("ReviewButton").onClick.AddListener(ReviewButton);
    }

    private void LoadAsset()
    {
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_settingNameBgSprite, GetUI<Image>("SettingNameBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_setFalseBgSprite, GetUI<Image>("SettingSetFalseBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_setFalseSprite, GetUI<Button>("SettingSetFalseButton").image, () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadOnlySprite(_buttonSprite, (sprite) => { 
            _clearLoadAssetCount++;
            GetUI<Image>("SetMusicButton").sprite = sprite;
            GetUI<Image>("ShowCreditButton").sprite = sprite;
            GetUI<Image>("ReviewButton").sprite = sprite;
        });


    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }

    private void SetSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            //변수 false로 변경
            PlayerController.Instance.PlayerData.IsSound = false;
            GpgsManager.Instance.SaveData((status) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    //사운드 끄기
                    _audio.Stop();
                    //Text 변환
                    _sb.Clear();
                    _sb.Append("사운드OFF");
                    GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
                }
            });
        }
        else if (PlayerController.Instance.PlayerData.IsSound == false)
        {
            //변수 false로 변경
            PlayerController.Instance.PlayerData.IsSound = true;
            GpgsManager.Instance.SaveData((status) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    //사운드 켜기
                    _audio.Play();
                    //Text 변환
                    _sb.Clear();
                    _sb.Append("사운드ON");
                    GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
                }
            });
        }

    }

    private void SetTrueCredit()
    {
        _CreditPanel.SetActive(true);
    }

    private void ReviewButton()
    {
        //string marketUrl = "market://details?id=" + _packageName;
        //string webUrl = "https://play.google.com/store/apps/details?id=" + _packageName;

        //try
        //{
        //    Application.OpenURL(marketUrl);
        //}
        //catch
        //{
        //    Application.OpenURL(webUrl);
        //}

        string testUrl = "https://play.google.com/store/apps/details?id=com.OJH.TheLastSun&hl=en-US&ah=ycKdRomcQYMd4i9uVCop1W5Jqgc&pli=1";
        Application.OpenURL(testUrl);

    }
}
