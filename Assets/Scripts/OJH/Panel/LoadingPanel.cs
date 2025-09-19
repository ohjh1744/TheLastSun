using Google.Play.Common.LoadingScreen;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.UI;

public enum IAssetLoad {Main, BossBook, Setting }
public class LoadingPanel : UIBInder
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private IAssetLoadable[] _assetLoadablePanels;

    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _boosBookPanel;
    [SerializeField] private GameObject _settingPanel;

    private Coroutine _routine;
    [SerializeField] private float _fakeLoadingTime;

    StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        _routine = StartCoroutine(CheckLoadAsset());
    }

    IEnumerator CheckLoadAsset()
    {
        //각 패널들 어드레서블 로드 완료되었는지 체크
        while (true)
        {
            bool isAnyNotClearLoad = false;

            for (int i = 0; i < _assetLoadablePanels.Length; i++)
            {
                // Panel들을 차례대로 탐색해서 Load클리어가 다 끝났는지 체크, 아니라면 종료하고 다시 재탐색
                if (_assetLoadablePanels[i].LoadAssetUICount > _assetLoadablePanels[i].ClearLoadAssetCount)
                {
                    isAnyNotClearLoad = true;
                    break;
                }
            }

            //다 클리어했다면 전체 반복문 종료
            if(isAnyNotClearLoad == false)
            {
                break;
            }

            yield return null;
        }


        //Fake Loading
        float time = 0f;
        while (time < _fakeLoadingTime)
        {
            Debug.Log("yeah");
            time += Time.deltaTime;
            GetUI<Slider>("LoadingSlider").value = time / _fakeLoadingTime;
            _sb.Clear();
            _sb.Append("Loading ");
            int percent = Mathf.FloorToInt(GetUI<Slider>("LoadingSlider").value * 100);
            if (percent == 100)
            {
                _sb.Append(99);
            }
            else
            {
                _sb.Append(percent);
            }
            _sb.Append("%");
            GetUI<TextMeshProUGUI>("LoadingText").SetText(_sb);
            yield return null;
        }

        // 다 되었따면 MainPanel 제외하고 다 꺼주기
        _boosBookPanel.SetActive(false);
        _settingPanel.SetActive(false);
        gameObject.SetActive(false);


    }

}
