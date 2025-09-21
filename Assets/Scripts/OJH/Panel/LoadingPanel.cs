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
    //어드레서블을 통해 불러와 적용할 에셋 개수
    private List<IAssetLoadable> _assetLoadablePanels;

    [SerializeField] private GameObject[] _panels;

    private Coroutine _routine;
    [SerializeField] private float _fakeLoadingTime;

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
        _assetLoadablePanels = new List<IAssetLoadable>();

        for(int i = 0; i < _panels.Length; i++)
        {
            IAssetLoadable panel = _panels[i].GetComponent<IAssetLoadable>();
            _assetLoadablePanels.Add(panel);
        }
    }

    IEnumerator CheckLoadAsset()
    {
        //이유는 불문명하지만 PC에서는 문제가 없으나 안드로이드에서 Awake -start 싱글톤 참조 문제 발생
        //초기화 될때가지 기다림
        Debug.Log("로드에셋체크 시작!!!!!");
        while (true)
        {
            if(AddressableManager.Instance != null && PlayerController.Instance != null && NetworkCheckManager.Instance != null && GpgsManager.Instance != null)
            {
                Debug.Log($"어드레서블매니저: {AddressableManager.Instance}");
                Debug.Log($"어드레서블매니저: {PlayerController.Instance} ");
                Debug.Log($"어드레서블매니저: {NetworkCheckManager.Instance} ");
                Debug.Log($"어드레서블매니저:  {GpgsManager.Instance}");
                break;
            }
            yield return null;
        }

        //패널들 모두 true.
        for(int i = 0; i < _panels.Length; i++)
        {
            _panels[i].SetActive(true);
        }

        Debug.Log("초기화 완료!");

        //각 패널들 어드레서블 로드 완료되었는지 체크
        while (true)
        {
            bool isAnyNotClearLoad = false;

            for (int i = 0; i < _assetLoadablePanels.Count; i++)
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
            time += Time.deltaTime;
            GetUI<Slider>("LoadingSlider").value = time / _fakeLoadingTime;
            _sb.Clear();
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

        //다 완료되면 MainPanel 제외하고 다 꺼주기
        for(int i = 0; i < _panels.Length; i++)
        {
            if(i == (int)IAssetLoad.Main)
            {
                continue;
            }
            _panels[i].SetActive(false);
        }
        gameObject.SetActive(false);
    }

}
