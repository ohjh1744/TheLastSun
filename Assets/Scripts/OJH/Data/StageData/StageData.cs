using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "StageData")]
public class StageData : ScriptableObject
{
    [SerializeField] private string _stageName;
    public string StageName { get { return _stageName; } private set { } }

    [SerializeField] private string _stageLevel;
    public string StageLevel { get { return _stageLevel; } private set { } }

    [SerializeField] private int _stageDifficulty;
    public int StageDifficulty { get { return _stageDifficulty; } private set { } }

    [SerializeField] private Color _unLockStageColor;
    public Color UnLockStageColor { get { return _unLockStageColor; } private set { } }

    [SerializeField] private Color _lockStageColor;
    public Color LockStageColor { get { return _lockStageColor; } private set { } }

}
