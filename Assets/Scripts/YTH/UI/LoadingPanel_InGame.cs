using UnityEngine;

public class LoadingPanel_InGame : MonoBehaviour
{
    [SerializeField] float waitTime = 3f;

    private void Start()
    {
        Invoke("StartGame", waitTime);
    }

    private void StartGame()
    {
        /* if (PlayerController.Instance.PlayerData.IsTutorial == false)
         {
            gameObject.SetActive(false);
         }*/

        GameManager.Instance.StartTimer();
        WaveManager.Instance.StartWave();
        gameObject.SetActive(false);
    }

}
