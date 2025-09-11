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

    [SerializeField] private AssetReferenceSprite _stageImageSprite;
    public AssetReferenceSprite StageImageSprite { get { return _stageImageSprite; } private set { } }
}
