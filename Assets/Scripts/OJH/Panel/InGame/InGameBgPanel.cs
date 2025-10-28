using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBgPanel : MonoBehaviour 
{
    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }
    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        
    }
}
