using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
public class SpecialSpawnPanel : UIBInder, IAssetLoadable
{
    private enum ESpecialSpawnUnit {Legend, Epic, God };

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

    [SerializeField] private AssetReferenceSprite _unitBgAndSpawnButtonBgSprite;

    [SerializeField] private AssetReferenceSprite _jemImageSprite;

    [SerializeField] private List<AssetReferenceSprite> _legendUnitSprites;

    [SerializeField] private List<AssetReferenceSprite> _epicUnitSprites;

    [SerializeField] private List<AssetReferenceSprite> _godUnitSprites;
    #endregion

    #region 저장해서 관리하는 에셋 혹은 UI

    private List<Sprite> _savedLegendUnitSprites;
    private List<Sprite> _savedEpicUnitSprites;
    private List<Sprite> _savedGodUnitSprites;
    private List<Sprite>[] savedUnitSprites;

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
        _savedLegendUnitSprites = new List<Sprite>();
        _savedEpicUnitSprites = new List<Sprite>();
        _savedGodUnitSprites = new List<Sprite>();
        savedUnitSprites = new List<Sprite>[] { _savedLegendUnitSprites, _savedEpicUnitSprites, _savedGodUnitSprites };

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

        AddressableManager.Instance.LoadSprite(_setFalseBgSprite, GetUI<Image>("SpecialSpawnlSetFalseButtonBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_setFalseSprite, GetUI<Button>("SpecialSpawnSetFalseButton").image, () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadSprite(_panelNameBgSprite, GetUI<Image>("SpecialSpawnNameBgImage"), () => { _clearLoadAssetCount++; });

        AddressableManager.Instance.LoadOnlySprite(_unitBgAndSpawnButtonBgSprite, (sprite) =>
        {
            _clearLoadAssetCount++;
            GetUI<Image>("LegendUnitSpecialSpawnBgImage").sprite = sprite;
            GetUI<Image>("EpicUnitSpecialSpawnBgImage").sprite = sprite;
            GetUI<Image>("GodUnitSpecialSpawnBgImage").sprite = sprite;
            GetUI<Image>("SpecialSpawnButtonImage").sprite = sprite;
        });

        LoadUnitSprites(_legendUnitSprites, _savedLegendUnitSprites, GetUI<Image>("LegendUnitSpecialSpawnPortraitImage"));
        LoadUnitSprites(_epicUnitSprites, _savedEpicUnitSprites, GetUI<Image>("EpicUnitSpecialSpawnPortraitImage"));
        LoadUnitSprites(_godUnitSprites, _savedGodUnitSprites, GetUI<Image>("GodUnitSpecialSpawnPortraitImage"));

        AddressableManager.Instance.LoadSprite(_jemImageSprite, GetUI<Image>("SpecialSpawnGemImage"), () => { _clearLoadAssetCount++; });
    }

    private void LoadUnitSprites(List<AssetReferenceSprite> unitSprites, List<Sprite> savedUnitSprites, Image image)
    {
        for(int i = 0; i < unitSprites.Count; i++)
        {
            int index = i;
            AddressableManager.Instance.LoadOnlySprite(unitSprites[index], (sprite) =>
            {
                _clearLoadAssetCount++;
                savedUnitSprites.Add(sprite);

                //초기에는 각 등급별 첫 유닛으로 초상화 초기화
                if (index == 0)
                {
                    image.sprite = sprite;
                }
            });
        }
    }
}
