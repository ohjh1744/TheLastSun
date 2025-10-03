using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 예시
/// "WarningPanel"만 활성화
/// inGameUIView.ShowOnlyPanel("WarningPanel");
/// 
/// "PausePanel" 활성화
/// inGameUIView.ShowPanel("PausePanel");
/// 
/// 모든 패널 비활성화
/// inGameUIView.HideAllPanels();
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public List<GameObject> Panels;

    private Dictionary<string, GameObject> _panelDict;

    //테스트용코드 추후 삭제
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowOnlyPanel("WarningPanel");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowOnlyPanel("GameOverPanel");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            HideAllPanels();
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _panelDict = new Dictionary<string, GameObject>();
        foreach (var panel in Panels)
        {
            if (panel != null)
            {
                _panelDict[panel.name] = panel;
                Debug.Log($"패널 등록: {panel.name}");
            }
        }
    }

    /// <summary>
    /// 패널 이름으로 활성화
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (_panelDict.TryGetValue(panelName, out var panel))
            panel.SetActive(true);
    }

    /// <summary>
    /// 패널 이름으로 비활성화
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (_panelDict.TryGetValue(panelName, out var panel))
            panel.SetActive(false);
    }

    /// <summary>
    /// 모든 패널 비활성화
    /// </summary>
    public void HideAllPanels()
    {
        foreach (var panel in _panelDict.Values)
            panel.SetActive(false);
    }

    /// <summary>
    /// 특정 패널만 활성화(나머지는 비활성화)
    /// </summary>
    public void ShowOnlyPanel(string panelName)
    {
        foreach (var kv in _panelDict)
            kv.Value.SetActive(kv.Key == panelName);
    }

    public void ShowPanelTemp(string panelName, float duration)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => ShowPanel(panelName))
                .AppendInterval(duration)
                .AppendCallback(() => HidePanel(panelName));
    }
}


