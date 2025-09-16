using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBInder
{
    //bgm AudioSource
    [SerializeField] private AudioSource _audio;

    [SerializeField] private GameObject _CreditPanel;

    StringBuilder _sb  = new StringBuilder();

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

        GetUI<Button>("SettingSetFalseButton").onClick.AddListener(SetFalsePanel);
        GetUI<Button>("SetFalseMusicButton").onClick.AddListener(SetFalseSound);
        GetUI<Button>("ShowCreditButton").onClick.AddListener(SetTrueCredit);
        GetUI<Button>("ReviewButton").onClick.AddListener(ReviewButton);
    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }

    private void SetFalseSound()
    {
        if (PlayerController.Instance.PlayerData.IsSound == true)
        {
            //사운드 끄기
            _audio.Stop();
            //Text 변환
            _sb.Clear();
            _sb.Append("사운드OFF");
            GetUI<TextMeshProUGUI>("SetFalseMusicButtonText").SetText(_sb);
            //변수 false로 변경
            PlayerController.Instance.PlayerData.IsSound = false;
        }
        else if (PlayerController.Instance.PlayerData.IsSound == false)
        {
            //사운드 켜기
            _audio.Play();
            //Text 변환
            _sb.Clear();
            _sb.Append("사운드ON");
            GetUI<TextMeshProUGUI>("SetFalseMusicButtonText").SetText(_sb);
            //변수 false로 변경
            PlayerController.Instance.PlayerData.IsSound = true;
        }

    }

    private void SetTrueCredit()
    {
        _CreditPanel.SetActive(true);
    }

    private void ReviewButton()
    {
        string marketUrl = "market://details?id=" + _packageName;
        string webUrl = "https://play.google.com/store/apps/details?id=" + _packageName;

        try
        {
            Application.OpenURL(marketUrl);
        }
        catch
        {
            Application.OpenURL(webUrl);
        }
    }
}
