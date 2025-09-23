using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeletePanel : MonoBehaviour
{
    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        GpgsManager.Instance.Login();
    }
    public void ResetData()
    {
        GpgsManager.Instance.DeleteData((callback) =>
        {
            if(callback == SavedGameRequestStatus.Success){
                _text.text = "데이터 삭제 성공";
            }
            else
            {
                _text.text = "데이터 삭제 실패";
            }
        });
    }

    public void GotoUpdatePanel()
    {
        gameObject.SetActive(false);
        _updatePanel.SetActive(true);
    }
}
