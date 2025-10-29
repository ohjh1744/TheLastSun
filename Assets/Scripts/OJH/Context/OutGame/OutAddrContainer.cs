using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class OutAddrContainer : UIBInder, IAssetLoadable
{
    #region IAssetLoadable 
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }
    #endregion

    #region Addressable Assets
    //UI
    [Header("UI")]
    [SerializeField] AssetReferenceSprite _button_02;
    [SerializeField] AssetReferenceSprite _button_03;
    [SerializeField] AssetReferenceSprite _button_04;
    [SerializeField] AssetReferenceSprite _cancel;
    [SerializeField] AssetReferenceSprite _frame;
    [SerializeField] AssetReferenceSprite _icon_Lock;
    [SerializeField] AssetReferenceSprite _popUp;
    [SerializeField] AssetReferenceSprite _popUp_Title;
    [SerializeField] List<AssetReferenceSprite> _backgroundSprites;
    [SerializeField] List<AssetReferenceSprite> _stagesSprites;
    [SerializeField] List<AssetReferenceSprite> _bossLocSprites;
    [SerializeField] List<AssetReferenceSprite> _bossUnLocSprites;
    #endregion

    void Awake()
    {
        BindAll();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
