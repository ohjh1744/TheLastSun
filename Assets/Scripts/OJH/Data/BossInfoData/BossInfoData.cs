using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "BossInfoData")]
public class BossInfoData : ScriptableObject
{
    [SerializeField] private string _bossName;
    public string BossName { get { return _bossName; } private set { } }

    [SerializeField] private string _bossShortInfo;
    public string BossShortInfo {  get { return _bossShortInfo;} private set { } }

    [SerializeField] private string _bossLongInfo;
    public string BossLongInfo {  get { return _bossLongInfo; } private set { } }

    [SerializeField] private AssetReferenceSprite _bossPortraitSprite;
    public AssetReferenceSprite BossPortraitSprite { get { return _bossPortraitSprite; } private set { } }

}
