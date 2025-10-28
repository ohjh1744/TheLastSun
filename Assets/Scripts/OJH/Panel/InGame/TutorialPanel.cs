using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TutorialPanel : UIBInder, IAssetLoadable
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
    //어드레서블
    [SerializeField] private AssetReferenceSprite[] _tutorialSprite;

    #endregion
    void Awake()
    {
        BindAll();
    }
    void Start()
    {
        Init();
    }

    private void Init()
    {
        AddEvent();
        LoadAsset();
    }
    private void AddEvent()
    {

    }

    private void LoadAsset()
    {

        for(int i = 0; i < _tutorialSprite.Length; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(_tutorialSprite[index], (sprite) => {
                _clearLoadAssetCount++;
                GetUI<Image>($"TutorialImage{index}").sprite = sprite;
            });
        }
    }


    //튜토리얼변수 클리어 함수

    //이미지 넘기기함수

    
}
