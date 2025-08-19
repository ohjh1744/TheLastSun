using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkErrorPanel : UIBInder
{
    [SerializeField] private Button _networkErrorRetryButton;
    [SerializeField] private Button _retryConnectOKButton;
    [SerializeField] private TextMeshProUGUI _retryConnectStatusText;
    [SerializeField] private GameObject _retryConnectPanel;

    Coroutine _routine;

    [SerializeField] private float _delayCheckConnect;

    WaitForSeconds _delayCheckConnectWs;

    StringBuilder _sb = new StringBuilder();

    private void Awake()
    {
        BindAll();
        //_delayCheckConnectWs = new WaitForSeconds(_delayCheckConnect);
    }

    private void Start()
    {
        _networkErrorRetryButton = GetUI<Button>("NetworkErrorRetryButton");
        _retryConnectOKButton = GetUI<Button>("RetryConnectOKButton");
        _retryConnectStatusText = GetUI<TextMeshProUGUI>("RetryConnectStatusText");

        _networkErrorRetryButton.onClick.AddListener(() => DoRetry());
        _retryConnectOKButton.onClick.AddListener(() => DoOK());
    }

    private void DoRetry()
    {
        if (_routine == null)
        {
            Debug.Log("두 리트라이");
            _routine = StartCoroutine(TryConnectNetwork());
        }
    }

    IEnumerator TryConnectNetwork()
    {
        NetworkCheckManager.Instance.DoCheckInternetInNotConnection();
        _retryConnectPanel.SetActive(true);

        Debug.Log("리트라이커넥페널 열림");

        Debug.Log($"네트워크네트워크1{NetworkCheckManager.Instance.IsConnected}");

        yield return new WaitForSeconds(2f);

        Debug.Log($"네트워크네트워크2{NetworkCheckManager.Instance.IsConnected }");

        if(NetworkCheckManager.Instance.IsConnected == true)
        {
            Debug.Log("연결 성공!");
            _retryConnectPanel.SetActive(false);
            gameObject.SetActive(false);
        }
        else if(NetworkCheckManager.Instance.IsConnected == false)
        {
            Debug.Log("연결 실패!");
            _sb.Clear();
            _sb.Append("인턴넷 연결에 실패했습니다.");
            _retryConnectStatusText.SetText(_sb);
            _retryConnectOKButton.gameObject.SetActive(true);
        }
        _routine = null;
    }

    private void DoOK()
    {
        Debug.Log("닫음!");
        _sb.Clear();
        _sb.Append("인터넷 연결 시도 중입니다.");
        //ok버튼 누를시 Text, 버튼 초기화해주고 Panel 꺼주기
        _retryConnectPanel.gameObject.SetActive(false);
        _retryConnectStatusText.SetText(_sb);
        _retryConnectOKButton.gameObject.SetActive(false);
    }
}
