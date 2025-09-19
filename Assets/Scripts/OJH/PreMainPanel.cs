using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum IAssetLoad {Main, BossBook, Setting }
public class PreMainPanel : MonoBehaviour
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private IAssetLoadable[] _assetLoadablePanels;

    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _boosBookPanel;
    [SerializeField] private GameObject _settingPanel;

    private Coroutine _routine;
    private WaitForSeconds _finishDelayWs;
    [SerializeField] private float _finishDelay;

    private void Awake()
    {
        _finishDelayWs = new WaitForSeconds(_finishDelay);
    }
    private void Start()
    {
        _routine = StartCoroutine(CheckLoadAsset());
    }

    IEnumerator CheckLoadAsset()
    {
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


        yield return _finishDelayWs;

        // 다 되었따면 MainPanel 제외하고 다 꺼주기
        _boosBookPanel.SetActive(false);
        _settingPanel.SetActive(false);
        gameObject.SetActive(false);


    }

}
