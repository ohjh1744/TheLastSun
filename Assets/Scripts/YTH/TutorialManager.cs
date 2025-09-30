using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialSteps;

    private int _currentStep = 0;

    public void StartTutorial()
    {
        Time.timeScale = 0f;
        _currentStep = 0;

        for (int i = 0; i < tutorialSteps.Length; i++)
            tutorialSteps[i].SetActive(i == 0);

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
            Time.timeScale = 1f;
            for (int i = 0; i < tutorialSteps.Length; i++)
                tutorialSteps[i].SetActive(false);
        }
    }
}
