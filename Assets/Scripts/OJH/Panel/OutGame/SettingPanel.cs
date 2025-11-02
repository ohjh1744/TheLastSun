using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingPanel : UIBInder
{

    //bgm AudioSource
    [SerializeField] private AudioSource _audio;

    [SerializeField] private GameObject _CreditPanel;

    StringBuilder _sb  = new StringBuilder();

    private string _packageName;

    private float _bgmTime;

    private void Awake()
    {
        BindAll();
        _packageName = Application.identifier;
    }

    private void OnEnable()
    {
        Init();
    }
    private void Start()
    {
        AddEvent();
    }

    private void Init()
    {
        //사운드 Text 초기화
        if (PlayerController.Instance.PlayerData.IsSound == true)
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
    }
    private void AddEvent()
    {
        GetUI<Button>("SettingSetFalseButton").onClick.AddListener(SetFalsePanel);
        GetUI<Button>("SetMusicButton").onClick.AddListener(SetSound);
        GetUI<Button>("ShowCreditButton").onClick.AddListener(SetTrueCredit);
        GetUI<Button>("ReviewButton").onClick.AddListener(ReviewButton);
    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }

    private void SetSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            if(NetworkCheckManager.Instance.IsConnected == true)
            {
                //변수 false로 변경 및 저장
                PlayerController.Instance.PlayerData.IsSound = false;
                GpgsManager.Instance.SaveData((success) => { });
                //사운드 끄기
                _audio.Pause();
                _bgmTime = _audio.time;
                //Text 변환
                _sb.Clear();
                _sb.Append("사운드OFF");
                GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
                Debug.Log($"사운드 off로 변환{PlayerController.Instance.PlayerData.IsSound}");
            }
        }
        else if (PlayerController.Instance.PlayerData.IsSound == false)
        {
            if(NetworkCheckManager.Instance.IsConnected == true)
            {
                //변수 true로 변경 및 저장
                PlayerController.Instance.PlayerData.IsSound = true;
                GpgsManager.Instance.SaveData((success) => { });
                //사운드 켜기
                _bgmTime = _audio.time;
                _audio.Play();
                //Text 변환
                _sb.Clear();
                _sb.Append("사운드ON");
                GetUI<TextMeshProUGUI>("SetMusicButtonText").SetText(_sb);
                Debug.Log($"사운드 ON으로 변환{PlayerController.Instance.PlayerData.IsSound}");
            }
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

        string testUrl = "https://play.google.com/store/apps/details?id=com.OJH.TheLastSun";
        Application.OpenURL(testUrl);

    }
}
