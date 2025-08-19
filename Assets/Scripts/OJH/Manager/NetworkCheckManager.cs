using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCheckManager : MonoBehaviour
{
    private static NetworkCheckManager _instance;
    public static NetworkCheckManager Instance { get  {return _instance; } set { _instance = value; } }

    private bool _isConnected;  // ���ͳ� ���� ���¸� ��Ÿ���� ����
    public bool IsConnected {  get { return _isConnected; } private set { } }

    [SerializeField] private float _checkRate;

    private WaitForSeconds _checkRateWs;

    [SerializeField] private GameObject _netWorkErrorPanel;

    public GameObject NetWorkErrorPanel { get { return _netWorkErrorPanel; } set { _netWorkErrorPanel = value; } }

    private Coroutine _CheckAlwaysRoutine;

    private Coroutine _CheckOnceRoutine;


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
        _checkRateWs = new WaitForSeconds(_checkRate);
        DoCheckInternetConnectionAlways();
    }

    private void Update()
    {
        //��Ʈ��ũ ������ �����Ǿ� ����г��� ��ٸ� ��Ʈ��ũ�׻�üŷ ����
        //_checkAlwaysRoutine������ ���� ������ NullReference ���� �������ؼ�
        if (_netWorkErrorPanel.activeSelf == true && _CheckAlwaysRoutine != null)
        {
            StopCoroutine(_CheckAlwaysRoutine);
            _CheckAlwaysRoutine = null;
        }
        // ��Ʈ��ũ ������ �ٽ� ����ȴٸ� ��Ʈ��ũ�׻�üŷ �ٽ� ���� 
        else if(_netWorkErrorPanel.activeSelf == false)
        {
            DoCheckInternetConnectionAlways();
        }
    }

    // �Ź� Ȯ��
    private void DoCheckInternetConnectionAlways ()
    {
        if (_CheckAlwaysRoutine == null)
        {
            _CheckAlwaysRoutine = StartCoroutine(CheckInternetConnectionAlways());
        }
    }

    //�ѹ��� Ȯ��
    // ��Ʈ��ũ ������ false�� ���¿��� �ٽ� ��Ʈ��ũ ���� Ȯ���ϴ� �Լ�
    public void DoCheckInternetInNotConnection()
    {
        if(_CheckOnceRoutine == null)
        {
            _CheckOnceRoutine = StartCoroutine(CheckInternetInNotConnection());
        }
    }

    IEnumerator CheckInternetConnectionAlways()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"); // ����: ���� ������

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _isConnected = true;
                Debug.Log("���ͳ� ���� Ȯ�ε�");
            }
            else
            {
                _isConnected = false;  // ���� ����
                _netWorkErrorPanel.SetActive(true);
                Debug.Log("���ͳ� ���� ����");
            }

            // isConnected ���� �α׷� ��� (����� �뵵)
            Debug.Log("���ͳ� ���� ����: " + _isConnected);

            yield return _checkRateWs;
        }
    }

    //��Ʈ��ũ������ ���¿��� �ٽ� üũ
    IEnumerator CheckInternetInNotConnection()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"); // ����: ���� ������

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            _isConnected = true;  // ������ ���¿��� �ٽ� ���� ����
            Debug.Log("���ͳ� ���� Ȯ�ε�");
        }
        else
        {
            _isConnected = false;  // ������ ���¿��� �� ���� ����
            Debug.Log("���ͳ� ���� ����");
        }

        // isConnected ���� �α׷� ��� (����� �뵵)
        Debug.Log("���ͳ� ���� ����: " + _isConnected);

        yield return _checkRateWs;

        _CheckOnceRoutine = null;
    }



}
