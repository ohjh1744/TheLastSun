using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobControlTower : MonoBehaviour
{

    private void Start()
    {
        
    }

    private void Update()
    {
        if (InGameManager.Instance.GameState != EGameState.Play)
            return;


        CheckWaveTime();
    }

    private void CheckWaveTime()
    {
        if (InGameManager.Instance.CurrentWaveTime <= 0.1f)
        {
            InGameManager.Instance.CurrentWaveTime = 0f;
            InGameManager.Instance.WaveNum++;
            InGameManager.Instance.CurrentWaveTime = InGameManager.Instance.WaveTimes[InGameManager.Instance.WaveNum];
        }

        InGameManager.Instance.CurrentWaveTime -= Time.deltaTime;
    }
}
