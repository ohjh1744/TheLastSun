using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BossData")]
public class BossData : MobData
{
    [SerializeField] private string _bossShortInfo;
    public string BossShortInfo { get { return _bossShortInfo; } set { _bossShortInfo = value; } }

    [SerializeField] private string _bossLongInfo;
    public string BossLongInfo { get { return _bossLongInfo; } set { _bossLongInfo = value; } }
}
