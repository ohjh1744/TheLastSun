using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class NetworkCheckManager : MonoBehaviour
{
    private static NetworkCheckManager _instance;
    public static NetworkCheckManager Instance { get  {return _instance; } set { _instance = value; } }

    private bool _isConnected;  // ���ͳ� ���� ���¸� ��Ÿ���� ����
    public bool IsConnected {  get { return _isConnected; } private set { } }

    [SerializeField] private GameObject _netWorkErrorPanel;

    public GameObject NetWorkErrorPanel { get { return _netWorkErrorPanel; } set { _netWorkErrorPanel = value; } }

    [SerializeField] private int _checkConnectRate;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DoCheckInternetConnectionAlways();
    }


    // �Ź� Ȯ��
    private async Task DoCheckInternetConnectionAlways ()
    {
        await CheckInternetConnectionAlwaysAsync();
    }


    async Task CheckInternetConnectionAlwaysAsync()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");

            // �񵿱� ��û�� ������ ����� ��ٸ�
            await SendWebRequestAsync(request);

            if (request.result == UnityWebRequest.Result.Success)
            {
                _isConnected = true;
                Debug.Log("���ͳ� ���� Ȯ�ε�");
            }
            else
            {
                _isConnected = false;
                _netWorkErrorPanel.SetActive(true);
                Debug.Log("���ͳ� ���� ����");
            }

            // isConnected ���� �α׷� ��� (����� �뵵)
            Debug.Log("���ͳ� ���� ����: " + _isConnected);

            // _checkRateWs ��ŭ ��� (Task.Delay�� ��ü)
            await Task.Delay(_checkConnectRate * 1000);
        }
    }

    // UnityWebRequest�� �񵿱� ó���� Task�� ���δ� �޼���
    private Task SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<object>(); // TaskCompletionSource ����

        // �� ��û�� ����
        request.SendWebRequest().completed += (op) =>
        {
            tcs.SetResult(null);
        };

        // ��û ��ȯ (�Ϸ�� ������ ��ٸ�)
        return tcs.Task;
    }

}
