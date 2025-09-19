using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    [SerializeField] private AssetReferenceSprite _stageImageSprite;
    [SerializeField] private AssetReferenceGameObject _test1Object;
    [SerializeField] private List<AssetReferenceGameObject> _test2Objects;

    //실제 저장되는 오브젝트
    [SerializeField] private GameObject _object;
    [SerializeField] private List<GameObject> _object1List;
    [SerializeField] private List<GameObject> _object2List;
    [SerializeField] private Image _stageImage;

    //리더보드 이름저장
    private List<string> _leaderboardString = new List<string>();

    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        //초기화
        _object1List = new List<GameObject>();
        _object2List = new List<GameObject>();
        _leaderboardString = new List<string>();
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_first_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_second_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_third_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_fourth_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_last_sun);


        //어드레서블 매니저를 활용한 오브젝트 생성 유형들, 이미지가져오기 등
        AddressableManager.Instance.GetObject(_test1Object, (result) =>
        {
            _object = result;
        });
        AddressableManager.Instance.GetObjectAndSave(_test1Object, _object1List, () => { });
        AddressableManager.Instance.GetObjectsAndSave(_test2Objects, _object2List, () => { });
        AddressableManager.Instance.LoadSprite(_stageImageSprite, _stageImage, () => { });
    }

    public void DoSaveData()
    {
        Debug.Log("SaveData하기");
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                //성공시 해야할 일
                //ex)팝업창
                Debug.Log("저장 성공!");
            }
            else
            {
                //실패시 해야할 일 
                //ex)팝업창
                Debug.Log("저장 실패!");
            }
        });
    }

    public void ShowAllLeaderbord()
    {
        Debug.Log("모든리더보기");
        GpgsManager.Instance.ShowAllLeaderboard();
    }

    public void InputLeaderbord()
    {
        Debug.Log("랭킹 넣기");
        //시간 , 원하는 리더보드 입력.
        GpgsManager.Instance.UpdateTimeLeaderboard(15000, _leaderboardString[0], (success) =>
        {
            if(success == true)
            {
                //성공시
            }
            else
            {
                //실패시
                //ex) 다시 시도
            }
        });
    }
}
