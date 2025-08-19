using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCheckManager : MonoBehaviour
{
    private static NetworkCheckManager _instance;
    public static NetworkCheckManager Instance { get  {return _instance; } set { _instance = value; } }

    private bool _isConnected;  // 인터넷 연결 상태를 나타내는 변수
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
        //네트워크 연결이 해제되어 경고패널이 뜬다면 네트워크항상체킹 중지
        //_checkAlwaysRoutine조건을 넣은 이유는 NullReference 에러 막기위해서
        if (_netWorkErrorPanel.activeSelf == true && _CheckAlwaysRoutine != null)
        {
            StopCoroutine(_CheckAlwaysRoutine);
            _CheckAlwaysRoutine = null;
        }
        // 네트워크 연결이 다시 연결된다면 네트워크항상체킹 다시 시작 
        else if(_netWorkErrorPanel.activeSelf == false)
        {
            DoCheckInternetConnectionAlways();
        }
    }

    // 매번 확인
    private void DoCheckInternetConnectionAlways ()
    {
        if (_CheckAlwaysRoutine == null)
        {
            _CheckAlwaysRoutine = StartCoroutine(CheckInternetConnectionAlways());
        }
    }

    //한번만 확인
    // 네트워크 연결이 false인 상태에서 다시 네트워크 연결 확인하는 함수
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
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"); // 예시: 구글 페이지

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _isConnected = true;
                Debug.Log("인터넷 연결 확인됨");
            }
            else
            {
                _isConnected = false;  // 연결 실패
                _netWorkErrorPanel.SetActive(true);
                Debug.Log("인터넷 연결 실패");
            }

            // isConnected 값을 로그로 출력 (디버깅 용도)
            Debug.Log("인터넷 연결 상태: " + _isConnected);

            yield return _checkRateWs;
        }
    }

    //네트워크에러뜬 상태에서 다시 체크
    IEnumerator CheckInternetInNotConnection()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"); // 예시: 구글 페이지

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            _isConnected = true;  // 에러뜬 상태에서 다시 연결 성공
            Debug.Log("인터넷 연결 확인됨");
        }
        else
        {
            _isConnected = false;  // 에러뜬 상태에서 또 연결 실패
            Debug.Log("인터넷 연결 실패");
        }

        // isConnected 값을 로그로 출력 (디버깅 용도)
        Debug.Log("인터넷 연결 상태: " + _isConnected);

        yield return _checkRateWs;

        _CheckOnceRoutine = null;
    }



}
