using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class NetworkErrorPanel : UIBInder
{
    [SerializeField] private GameObject _retryConnectPanel;

    [SerializeField] private int _tryConnectDelay;

    StringBuilder _sb = new StringBuilder();

    private bool _isTryConnect;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        // 정지
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        //시간 다시 켜기
        Time.timeScale = 1f;
    }

    private void Start()
    {
        GetUI<Button>("NetworkErrorRetryButton").onClick.AddListener(() => DoRetry());
        GetUI<Button>("RetryConnectOKButton").onClick.AddListener(() => CloseRetryConnectPanel());
    }

    //네트워크 재연결 시도 하기
    private async Task DoRetry()
    {
        if(_isTryConnect == false)
        {
            await TryConnectNetworkAsync();
        }
    }
    async Task TryConnectNetworkAsync()
    {
        _isTryConnect = true;
        _retryConnectPanel.SetActive(true);

        Debug.Log("리트라이커넥페널 열림");

        Debug.Log($"네트워크네트워크1 {NetworkCheckManager.Instance.IsConnected}");

        // 비동기 대기 (예: 딜레이 후 체크)
        // second 기준으로학이위하여 1000곱함
        await Task.Delay(_tryConnectDelay * 1000);

        Debug.Log($"네트워크네트워크2 {NetworkCheckManager.Instance.IsConnected}");

        if (NetworkCheckManager.Instance.IsConnected)
        {
            Debug.Log("연결 성공!");
            //패널들 다 꺼주기
            _retryConnectPanel.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("연결 실패!");
            _sb.Clear();
            _sb.Append("인터넷 연결에 실패했습니다.");
            //돌아가기 OK 버튼 활성화
            GetUI<TextMeshProUGUI>("RetryConnectStatusText").SetText(_sb);
            GetUI<Button>("RetryConnectOKButton").gameObject.SetActive(true);
        }

        _isTryConnect = false;

    }

    //네트워크 연결실패 후 RetryConnectPanel을 끄기 위한 함수
    private void CloseRetryConnectPanel()
    {
        Debug.Log("닫음!");
        _sb.Clear();
        _sb.Append("인터넷 연결 시도 중입니다.");
        //ok버튼 누를시 Text, 버튼 초기화해주고 Panel 꺼주기
        _retryConnectPanel.gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("RetryConnectStatusText").SetText(_sb);
        GetUI<Button>("RetryConnectOKButton").gameObject.SetActive(false);
    }
}
