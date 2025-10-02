using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    private InGameUI _uiList;

    [HideInInspector] public List<GameObject> Panels;

    private Dictionary<string, GameObject> _panelDict;

    private void Start()
    {
        _panelDict = new Dictionary<string, GameObject>();
        foreach (var panel in Panels)
        {
            if (panel != null)
                _panelDict[panel.name] = panel;
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
}


