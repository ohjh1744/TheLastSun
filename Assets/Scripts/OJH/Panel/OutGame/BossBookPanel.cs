using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BossBookPanel : UIBInder
{

    [SerializeField] private GameObject _bossBookExplainContext; // 버튼 누를 시 활성화

    [SerializeField] private BossInfoData[] _bossInfoDatas;

    //보스 버튼 LIst에 저장
    private List<Button> _bossPortraitButton = new List<Button>();

    private StringBuilder _sb = new StringBuilder();

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
    }

    //자주 사용하는 UI 가져오고 저장
    private void GetUI()
    {
        for(int i = 0; i < _bossInfoDatas.Length; i++)
        {
            _bossPortraitButton.Add(GetUI<Button>($"BossPortraitButton{i+1}"));
        }

        for (int i = 0; i < _bossInfoDatas.Length; i++)
        {
            _bossPortraitSprites.Add(GetUI<Image>($"BossPortraitButton{i + 1}").sprite);
            Debug.Log(GetUI<Image>($"BossPortraitButton{i + 1}").sprite);
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
        Debug.Log(_bossPortraitSprites[bossIndex]);

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
