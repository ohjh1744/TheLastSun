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
            Debug.Log("�� ��Ʈ����");
            _routine = StartCoroutine(TryConnectNetwork());
        }
    }

    IEnumerator TryConnectNetwork()
    {
        NetworkCheckManager.Instance.DoCheckInternetInNotConnection();
        _retryConnectPanel.SetActive(true);

        Debug.Log("��Ʈ����Ŀ����� ����");

        Debug.Log($"��Ʈ��ũ��Ʈ��ũ1{NetworkCheckManager.Instance.IsConnected}");

        yield return new WaitForSeconds(2f);

        Debug.Log($"��Ʈ��ũ��Ʈ��ũ2{NetworkCheckManager.Instance.IsConnected }");

        if(NetworkCheckManager.Instance.IsConnected == true)
        {
            Debug.Log("���� ����!");
            _retryConnectPanel.SetActive(false);
            gameObject.SetActive(false);
        }
        else if(NetworkCheckManager.Instance.IsConnected == false)
        {
            Debug.Log("���� ����!");
            _sb.Clear();
            _sb.Append("���ϳ� ���ῡ �����߽��ϴ�.");
            _retryConnectStatusText.SetText(_sb);
            _retryConnectOKButton.gameObject.SetActive(true);
        }
        _routine = null;
    }

    private void DoOK()
    {
        Debug.Log("����!");
        _sb.Clear();
        _sb.Append("���ͳ� ���� �õ� ���Դϴ�.");
        //ok��ư ������ Text, ��ư �ʱ�ȭ���ְ� Panel ���ֱ�
        _retryConnectPanel.gameObject.SetActive(false);
        _retryConnectStatusText.SetText(_sb);
        _retryConnectOKButton.gameObject.SetActive(false);
    }
}
