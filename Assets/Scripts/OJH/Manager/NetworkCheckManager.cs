using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.Events;

public class NetworkCheckManager : MonoBehaviour
{
    private static NetworkCheckManager _instance;
    public static NetworkCheckManager Instance { get  {return _instance; } set { _instance = value; } }

    //게임자체에서 인터넷 연결확인 변수
    private bool _isConnected;  
    public bool IsConnected {  get { return _isConnected; }  set { _isConnected = value; } }

    //실제 인터넷 연결 확인
    private bool _isCurConnected;
    public bool IsCurConnected { get { return _isCurConnected; } private set { } }

    [SerializeField] private GameObject _netWorkErrorPanel;

    public GameObject NetWorkErrorPanel { get { return _netWorkErrorPanel; } set { _netWorkErrorPanel = value; } }

    [SerializeField] private int _checkConnectRate;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _isConnected = true;
        _isCurConnected = true;
        DoCheckInternetConnectionAlways();
    }


    // 매번 확인
    private async Task DoCheckInternetConnectionAlways ()
    {
        await CheckInternetConnectionAlwaysAsync();
    }


    async Task CheckInternetConnectionAlwaysAsync()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");

            // 비동기 요청을 보내고 결과를 기다림
            await SendWebRequestAsync(request);

            if (request.result == UnityWebRequest.Result.Success)
            {
                _isCurConnected = true;
                Debug.Log("인터넷 연결 확인됨");
            }
            else
            {
                _isCurConnected = false;
                _netWorkErrorPanel.SetActive(true);
                Debug.Log("인터넷 연결 실패");
            }

            // isConnected 값을 로그로 출력 (디버깅 용도)
            Debug.Log("실제 인터넷 연결 상태: " + _isCurConnected);
            Debug.Log("게임내에서 체크한 인터넷 연결 상태: " + _isConnected);

            // _checkRateWs 만큼 대기 (Task.Delay로 대체)
            await Task.Delay(_checkConnectRate * 1000);
        }
    }

    // UnityWebRequest의 비동기 처리를 Task로 감싸는 메서드
    private Task SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<object>(); // TaskCompletionSource 생성

        // 웹 요청을 보냄
        request.SendWebRequest().completed += (op) =>
        {
            tcs.SetResult(null);
        };

        // 요청 반환 (완료될 때까지 기다림)
        return tcs.Task;
    }

}
