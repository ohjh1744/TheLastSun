using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    [SerializeField] private float[] clearTimes;
    public float[] ClearTimes { get { return clearTimes; } set {clearTimes = value; } }
}
