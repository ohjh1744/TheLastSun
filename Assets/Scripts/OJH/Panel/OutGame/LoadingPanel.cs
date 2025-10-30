using Google.Play.Common.LoadingScreen;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum IAssetLoad {Main, BossBook, Setting, Credit }
public class LoadingPanel : UIBInder
{
    private IAssetLoadable _assetLoadableObject;

    //어드레서블을 통해 불러와 에셋 적용하는 패널들
    [SerializeField] private GameObject _assetLoadObject;

    //준비 완료 이후, false할 패널들과 True할 패널들
    [SerializeField] private GameObject[] _setFalsePanels;
    [SerializeField] private GameObject[] _setTruePanels;

    private Coroutine _routine;

    StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        Init();
        _routine = StartCoroutine(CheckLoadAsset());
    }

    private void Init()
    {
        IAssetLoadable panel = _assetLoadObject.GetComponent<IAssetLoadable>();
        _assetLoadableObject = panel;
        Debug.Log(panel);
    }

    IEnumerator CheckLoadAsset()
    {
        Debug.Log("부드러운 실제 로딩 시작!");

        float displayedProgress = 0f;
        float lerpSpeed = 2f; // UI가 따라가는 속도
        StringBuilder sb = new StringBuilder();

        int totalAssets = Mathf.Max(_assetLoadableObject.LoadAssetUICount, 1); // 0 방지

        while (displayedProgress < 1f)
        {
            Debug.Log($"{_assetLoadableObject.LoadAssetUICount}");
            Debug.Log($"{_assetLoadableObject.ClearLoadAssetCount}");
            // 실제 진행률 계산
            float realProgress = (float)_assetLoadableObject.ClearLoadAssetCount / totalAssets;

            // Lerp로 부드럽게 증가 (deltaTime을 곱해서 프레임 독립적)
            displayedProgress = Mathf.Lerp(displayedProgress, realProgress, Time.deltaTime * lerpSpeed);

            // Slider & Text 업데이트
            GetUI<Slider>("LoadingSlider").value = displayedProgress;
            sb.Clear();
            int percent = Mathf.FloorToInt(displayedProgress * 100);
            sb.Append(percent);
            sb.Append("%");
            GetUI<TextMeshProUGUI>("LoadingText").SetText(sb);

            // 종료 조건: 실제 로딩 완료 + UI가 거의 100%
            if (_assetLoadableObject.ClearLoadAssetCount >= totalAssets && displayedProgress >= 0.99f)
                break;

            yield return null;
        }

        // 100% 보정
        GetUI<Slider>("LoadingSlider").value = 1f;
        GetUI<TextMeshProUGUI>("LoadingText").SetText("100%");

        // 패널 처리
        foreach (var panel in _setFalsePanels)
            panel.SetActive(false);

        foreach (var panel in _setTruePanels)
            panel.SetActive(true);

        Debug.Log("실제 로딩 완료!");
    }
}
