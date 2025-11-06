using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialPanel : UIBInder
{

    [SerializeField] private int _tutorialMaxIndex;

    private int _tutorialIndex;

    WaitForSeconds _wsDelay;

    void Awake()
    {
        BindAll();
        _wsDelay = new WaitForSeconds(0.1f);
    }

    private void OnEnable()
    {
        Init();
    }

   
    void Start()
    {
        DoAddEvent();
    }

    private void Init()
    {
        //IsToturial false인 경우에만 해주기
        if (PlayerController.Instance.PlayerData.IsTutorial == false)
        {
            SetIsTutorialTrue();
        }

        //튜토리얼 이미지 초기화
        for (int i = 0; i < _tutorialMaxIndex; i++)
        {
            if (i == 0)
            {
                GetUI($"TutorialImage{i}").SetActive(true);
            }
            else
            {
                GetUI($"TutorialImage{i}").SetActive(false);
            }
        }
    }

    private void DoAddEvent()
    {
        for(int i = 0; i < _tutorialMaxIndex; i++)
        {
            AddEvent($"TutorialImage{i}", EventType.Click, DoNextTutorial);
        }
    }

    private void DoNextTutorial(PointerEventData eventData)
    {
        _tutorialIndex++;

        if(_tutorialIndex == _tutorialMaxIndex)
        {
            for (int i = 0; i < _tutorialMaxIndex; i++)
            {
                GetUI($"TutorialImage{i}").SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _tutorialMaxIndex; i++)
            {
                if (_tutorialIndex == i)
                {
                    GetUI($"TutorialImage{i}").SetActive(true);
                }
                else if (_tutorialIndex != i)
                {
                    GetUI($"TutorialImage{i}").SetActive(false);
                }
            }
        }

    }

    private void SetIsTutorialTrue()
    {
        StartCoroutine(OnSetIsTutorialTrue());
    }

    IEnumerator OnSetIsTutorialTrue()
    {
        while(NetworkCheckManager.Instance.IsConnected == false)
        {
            yield return _wsDelay;
        }

        GpgsManager.Instance.SaveData((success) =>{});
    }



    //튜토리얼변수 클리어 함수

    //이미지 넘기기함수

    
}
