using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CreditPanel : UIBInder, IAssetLoadable
{
    //어드레서블을 통해 불러와 적용할 에셋 개수
    [SerializeField] private int _loadAssetUICount;
    public int LoadAssetUICount { get { return _loadAssetUICount; } set { _loadAssetUICount = value; } }

    //현재 어드레서블을 통해 적용끝난 에셋 개수
    private int _clearLoadAssetCount;
    public int ClearLoadAssetCount { get { return _clearLoadAssetCount; } set { _clearLoadAssetCount = value; } }


    //어드레서블

    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _setFalseBgSprite;

    [SerializeField] private AssetReferenceSprite _setFalseSprite;

    private void Awake()
    {
        BindAll();
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadAsset();
        AddEvent();
    }

    private void LoadAsset()
    {
        Image image = GetComponent<Image>();
        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_setFalseBgSprite, GetUI<Image>("CreditSetFalseBgImage"), () => { _clearLoadAssetCount++; });
        AddressableManager.Instance.LoadSprite(_setFalseSprite, GetUI<Button>("CreditSetFalseButton").image, () => { _clearLoadAssetCount++; });

    }

    private void AddEvent()
    {
        GetUI<Button>("CreditSetFalseButton").onClick.AddListener(SetFalsePanel);
    }

    private void SetFalsePanel()
    {
        gameObject.SetActive(false);
    }
}
