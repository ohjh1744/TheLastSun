using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : UIBInder
{
    private Button _stopButton => GetUI("StopButton").GetComponent<Button>();

    private void Awake()
    {
        Bind();

        _stopButton.clicked += () => GameManager.Instance.SetGameSpeed(0);
    }
}
