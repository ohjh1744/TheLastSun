using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetLoadable 
{
    public int LoadAssetUICount {get; set;}
    public int ClearLoadAssetCount { get;  set; }
}
