using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class NetworkErrorPanel : UIBInder
{
    [SerializeField] private Button _networkErrorRetryButton;
    [SerializeField] private Button _retryConnectOKButton;
    [SerializeField] private TextMeshProUGUI _retryConnectStatusText;
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
        // ����
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        //�ð� �ٽ� �ѱ�
        Time.timeScale = 1f;
    }

    private void Start()
    {
        _networkErrorRetryButton = GetUI<Button>("NetworkErrorRetryButton");
        _retryConnectOKButton = GetUI<Button>("RetryConnectOKButton");
        _retryConnectStatusText = GetUI<TextMeshProUGUI>("RetryConnectStatusText");

        _networkErrorRetryButton.onClick.AddListener(() => DoRetry());
        _retryConnectOKButton.onClick.AddListener(() => CloseRetryConnectPanel());
    }

    //��Ʈ��ũ �翬�� �õ� �ϱ�
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

        Debug.Log("��Ʈ����Ŀ����� ����");

        Debug.Log($"��Ʈ��ũ��Ʈ��ũ1 {NetworkCheckManager.Instance.IsConnected}");

        // �񵿱� ��� (��: ������ �� üũ)
        // second ���������������Ͽ� 1000����
        await Task.Delay(_tryConnectDelay * 1000);

        Debug.Log($"��Ʈ��ũ��Ʈ��ũ2 {NetworkCheckManager.Instance.IsConnected}");

        if (NetworkCheckManager.Instance.IsConnected)
        {
            Debug.Log("���� ����!");
            //�гε� �� ���ֱ�
            _retryConnectPanel.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("���� ����!");
            _sb.Clear();
            _sb.Append("���ͳ� ���ῡ �����߽��ϴ�.");
            //���ư��� OK ��ư Ȱ��ȭ
            _retryConnectStatusText.SetText(_sb);
            _retryConnectOKButton.gameObject.SetActive(true);
        }

        _isTryConnect = false;

    }

    //��Ʈ��ũ ������� �� RetryConnectPanel�� ���� ���� �Լ�
    private void CloseRetryConnectPanel()
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
