using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    // 패널에 버튼 컴포넌트가 있어야 함
    [SerializeField] GameObject[] tutorialSteps;

    private int _currentStep = 0;

    [SerializeField] LoadingPanel_InGame _loadingPanel_InGame;

    private void Awake()
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(false);
        }
    }

    private void Start()
    {
        /*if (!PlayerController.Instance.PlayerData.IsTutorial)
        {
            StartTutorial();
        }
        else
        {
            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                tutorialSteps[i].SetActive(false);
            }
        }*/
    }

    public void SetTutorial()
    {
        PlayerController.Instance.PlayerData.IsTutorial = false; // Test용 임시 코드
        if (!PlayerController.Instance.PlayerData.IsTutorial)
        {
            StartTutorial();
        }
        else
        {
            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                tutorialSteps[i].SetActive(false);
            }

            _loadingPanel_InGame.StartGame();
        }
    }

    public void StartTutorial()
    {
        Time.timeScale = 0f;
        _currentStep = 0;

        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].SetActive(i == 0);
        }
        AddClickEvent(tutorialSteps[0]);
    }

    private void AddClickEvent(GameObject panel)
    {
        Button btn = panel.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(NextStep);
        }
    }

    private void NextStep()
    {
        tutorialSteps[_currentStep].SetActive(false);
        _currentStep++;

        if (_currentStep < tutorialSteps.Length)
        {
            tutorialSteps[_currentStep].SetActive(true);
            AddClickEvent(tutorialSteps[_currentStep]);
        }

        else
        {
            // 튜토리얼 종료
            PlayerController.Instance.PlayerData.IsTutorial = true;
            _loadingPanel_InGame.StartGame();//테스트용 임시코드
#if !UNITY_EDITOR
            StartCoroutine(WaitForNetworkAndSave());
#endif
        }
    }

    private IEnumerator WaitForNetworkAndSave()
    {
        // 네트워크 연결될 때까지 0.5초 간격으로 대기
        while (!NetworkCheckManager.Instance.IsConnected)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 연결되었으므로 저장 진행
        GpgsManager.Instance.SaveData((status) =>
        {
            if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
            {
                Time.timeScale = 1f;
                for (int i = 0; i < tutorialSteps.Length; i++)
                {
                    tutorialSteps[i].SetActive(false);
                }

                _loadingPanel_InGame.StartGame();
            }
            else
            {
                Debug.Log("네트워크 연결 실패...");
                // TODO: 재시도 로직 등
            }
        });
    }
}
