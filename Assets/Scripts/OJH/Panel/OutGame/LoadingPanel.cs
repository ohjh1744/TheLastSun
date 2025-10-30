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
    }

    IEnumerator CheckLoadAsset()
    {
        Debug.Log("부드러운 실제 로딩 시작!");

        float displayedProgress = 0f;       // UI에 보여줄 현재 진행률 (0~1)
        float lerpSpeed = 2f;               // 보간 속도 (값이 클수록 빠르게 올라감)
        float minLoadingTime = 2f;          // 최소 로딩 시간(초)
        float elapsedTime = 0f;

        int totalAssets = Mathf.Max(_assetLoadableObject.LoadAssetUICount, 1); // 0 방지

        while (displayedProgress < 1f)
        {
            // 실제 진행률
            float realProgress = (float)_assetLoadableObject.ClearLoadAssetCount / totalAssets;

            // 최소 시간 기준으로 강제로 부드럽게 증가
            float minTimeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);

            // 실제 진행률과 최소시간 진행률 중 큰 쪽으로 UI 갱신
            float targetProgress = Mathf.Max(realProgress, minTimeProgress);

            // 부드럽게 MoveTowards로 증가
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * lerpSpeed);

            // Slider & Text 업데이트
            GetUI<Slider>("LoadingSlider").value = displayedProgress;
            _sb.Clear();
            int percent = Mathf.FloorToInt(displayedProgress * 100);
            _sb.Append(percent);
            _sb.Append("%");
            GetUI<TextMeshProUGUI>("LoadingText").SetText(_sb);

            // 종료 조건: 실제 로딩 완료 + UI가 100% 도달
            if (_assetLoadableObject.ClearLoadAssetCount >= totalAssets && displayedProgress >= 1f)
                break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 100% 보정
        GetUI<Slider>("LoadingSlider").value = 1f;
        GetUI<TextMeshProUGUI>("LoadingText").SetText("100%");

        // 패널 처리
        for (int i = 0; i < _setFalsePanels.Length; i++)
            _setFalsePanels[i].SetActive(false);

        for (int i = 0; i < _setTruePanels.Length; i++)
            _setTruePanels[i].SetActive(true);

        Debug.Log("실제 로딩 완료!");
    }

}
