using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SellPanel : UIBInder, IAssetLoadable
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
    [SerializeField] private AssetReferenceSprite _bgImageSprite;

    [SerializeField] private AssetReferenceSprite _setFalseBgSprite;

    [SerializeField] private AssetReferenceSprite _setFalseSprite;

    [SerializeField] private AssetReferenceSprite _panelNameBgSprite;

    [SerializeField] private AssetReferenceSprite _unitBgSprite;

    [SerializeField] private List<AssetReferenceSprite> _warriorUnitSprites;

    [SerializeField] private List<AssetReferenceSprite> _archerUnitSprites;

    [SerializeField] private List<AssetReferenceSprite> _bomerUnitSprites;

    [SerializeField] private AssetReferenceSprite _jemImageSprite;

    [SerializeField] private AssetReferenceSprite _changeSellUnitBgSprite;

    #endregion

    #region 저장해서 관리하는 에셋 혹은 UI

    private List<Sprite> _savedWarriorUnitSprites;
    private List<Sprite> _savedArcherUnitSprites;
    private List<Sprite> _savedBomerUnitSprites;
    private List<Sprite>[] _savedUnitSprites;

    private List<Image> _savedUnitSellPortraitImages;
    private List<Image> _savedGetJemImages;
    private List<Image> _savedChangedSellBgImages;

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
        _savedWarriorUnitSprites = new List<Sprite>();
        _savedArcherUnitSprites = new List<Sprite>();
        _savedBomerUnitSprites = new List<Sprite>();
        _savedUnitSprites = new List<Sprite>[]{ _savedWarriorUnitSprites, _savedArcherUnitSprites, _savedBomerUnitSprites };
        _savedUnitSellPortraitImages = new List<Image> { GetUI<Image>("NormalUnitSellPortraitImage"), GetUI<Image>("RareUnitSellPortraitImage"), GetUI<Image>("AncientUnitSellPortraitImage"), GetUI<Image>("LegendUnitSellPortraitImage"), GetUI<Image>("EpicUnitSellPortraitImage"), };
        _savedGetJemImages = new List<Image> { GetUI<Image>("GetNormalJemImage"), GetUI<Image>("GetRareJemImage"), GetUI<Image>("GetAncientJemImage"), GetUI<Image>("GetLegendJemImage"), GetUI<Image>("GetEpicJemImage") };
        _savedChangedSellBgImages = new List<Image> { GetUI<Image>("ChangeSellWarriorButton"), GetUI<Image>("ChangeSellArcherButton"), GetUI<Image>("ChangeSellBomerButton") };

        AddEvent();
        LoadAsset();
    }
    private void AddEvent()
    {

    }

    private void LoadAsset()
    {
        Image image = GetComponent<Image>();

        AddressableManager.Instance.LoadSprite(_bgImageSprite, image, () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_setFalseBgSprite, GetUI<Image>("SellPanelSetFalseButtonBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_setFalseSprite, GetUI<Button>("SellPanelSetFalseButton").image, () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_panelNameBgSprite, GetUI<Image>("SellPanelNameBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadOnlySprite(_unitBgSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("NormalUnitSellBgImage").sprite = sprite;
            GetUI<Image>("RareUnitSellBgImage").sprite = sprite;
            GetUI<Image>("AncientUnitSellBgImage").sprite = sprite;
            GetUI<Image>("LegendUnitSellBgImage").sprite = sprite;
            GetUI<Image>("EpicUnitSellBgImage").sprite = sprite;
        });

        LoadUnitSprites(true, _warriorUnitSprites, _savedWarriorUnitSprites);
        LoadUnitSprites(false, _archerUnitSprites, _savedArcherUnitSprites);
        LoadUnitSprites(false, _bomerUnitSprites, _savedBomerUnitSprites);

        AddressableManager.Instance.LoadOnlySprite(_jemImageSprite, (sprite) =>
        {
            for(int i = 0; i < _savedGetJemImages.Count; i++)
            {
                _clearLoadAssetCount++;
                _savedGetJemImages[i].sprite = sprite;
            }
        });

        AddressableManager.Instance.LoadOnlySprite(_changeSellUnitBgSprite, (sprite) =>
        {
            for (int i = 0; i < _savedChangedSellBgImages.Count; i++)
            {
                _clearLoadAssetCount++;
                _savedChangedSellBgImages[i].sprite = sprite;
            }
        });
    }

    private void LoadUnitSprites(bool isInit, List<AssetReferenceSprite> unitSprites, List<Sprite> savedUnitSprites)
    {
        for (int i = 0; i < unitSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(unitSprites[index], (sprite) =>
            {
                _clearLoadAssetCount++;
                savedUnitSprites.Add(sprite);
                //초기화 경우
                if (isInit == true)
                {
                    _savedUnitSellPortraitImages[index].sprite = sprite;
                }
            });
        }
    }
}
