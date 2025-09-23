using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class JsonTest : MonoBehaviour
{
    public void Save()
    {
        StringBuilder path = new StringBuilder();
        path.Append(Application.persistentDataPath).Append("/Save");
        if (Directory.Exists(path.ToString()) == false)
        {
            Directory.CreateDirectory(path.ToString());
        }

        string json = JsonUtility.ToJson(PlayerController.Instance.PlayerData);
        File.WriteAllText($"{path}/{"savesave"}.txt", json);
    }

    public void Load()
    {
        StringBuilder path = new StringBuilder();
        path.Append(Application.persistentDataPath).Append($"/Save/{"savesave"}.txt");
        if (File.Exists(path.ToString()) == false)
        {
            Debug.Log("Cant Find Save File");
            return;
        }
        string json = File.ReadAllText(path.ToString());
        PlayerData _oldData = JsonUtility.FromJson<PlayerData>(json);

        //이런식으로 
        for(int i = 0; i < PlayerController.Instance.PlayerData.ClearTimes.Count; i++)
        {
            if(i < _oldData.ClearTimes.Count)
            {
                PlayerController.Instance.PlayerData.ClearTimes[i] = _oldData.ClearTimes[i];
            }
            else
            {
                PlayerController.Instance.PlayerData.ClearTimes[i] = 0;
            }
        }

        Debug.Log(json);
        PlayerController.Instance.PlayerData = _oldData;
        Debug.Log(JsonUtility.ToJson(PlayerController.Instance.PlayerData));

        Debug.Log(path);

    }
}
