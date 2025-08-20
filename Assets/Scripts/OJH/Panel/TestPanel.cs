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

    //���� ����Ǵ� ������Ʈ
    [SerializeField] private GameObject _object;
    [SerializeField] private List<GameObject> _object1List;
    [SerializeField] private List<GameObject> _object2List;
    [SerializeField] private Image _stageImage;

    //�������� �̸�����
    private List<string> _leaderboardString = new List<string>();

    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        //�ʱ�ȭ
        _object1List = new List<GameObject>();
        _object2List = new List<GameObject>();
        _leaderboardString = new List<string>();
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_first_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_second_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_third_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_fourth_sun);
        _leaderboardString.Add(GPGSIds.leaderboard_clear_time_of_the_last_sun);


        //��巹���� �Ŵ����� Ȱ���� ������Ʈ ���� ������, �̹����������� ��
        AddressableManager.Instance.GetObject(_test1Object, (result) =>
        {
            _object = result;
        });
        AddressableManager.Instance.GetObjectAndSave(_test1Object, _object1List);
        AddressableManager.Instance.GetObjectsAndSave(_test2Objects, _object2List);
        AddressableManager.Instance.LoadSprite(_stageImageSprite, _stageImage);
    }

    public void DoSaveData()
    {
        Debug.Log("SaveData�ϱ�");
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                //������ �ؾ��� ��
                //ex)�˾�â
                Debug.Log("���� ����!");
            }
            else
            {
                //���н� �ؾ��� �� 
                //ex)�˾�â
                Debug.Log("���� ����!");
            }
        });
    }

    public void ShowAllLeaderbord()
    {
        Debug.Log("��縮������");
        GpgsManager.Instance.ShowAllLeaderboard();
    }

    public void InputLeaderbord()
    {
        Debug.Log("��ŷ �ֱ�");
        //�ð� , ���ϴ� �������� �Է�.
        GpgsManager.Instance.UpdateTimeLeaderboard(15000, _leaderboardString[0]);
    }
}
