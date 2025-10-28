using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public enum EUnit {Warrior, Archer, Bomer}
public enum EGod {Nana, Tona, Xiu }
public class UnitPool : MonoBehaviour
{
    //어드레서블로 불러온 프리펩 저장하는 공간
    private List<GameObject> _savedNormalUnits;
    public List<GameObject> SavedNormalUnits { get { return _savedNormalUnits; } set { _savedNormalUnits = value; } }

    private List<GameObject> _savedRareUnits;
    public List<GameObject> SavedRareUnits { get { return _savedRareUnits; } set { _savedRareUnits = value; } }

    private List<GameObject> _savedAncientUnits;
    public List<GameObject> SavedAncientUnits { get { return _savedAncientUnits; } set { _savedAncientUnits = value; } }

    private List<GameObject> _savedLegendUnits;
    public List<GameObject> SavedLegendUnits { get { return _savedLegendUnits; } set { _savedLegendUnits = value; } }

    private List<GameObject> _savedEpicUnits;
    public List<GameObject> SavedEpicUnits { get { return _savedEpicUnits; } set { _savedEpicUnits = value; } }

    private List<GameObject> _savedGodUnits;
    public List<GameObject> SavedGodUnits { get { return _savedGodUnits; } set { _savedGodUnits = value; } }

    //게임 내에서 사용하는 실제 Pools
    private List<GameObject>[] _normalUnitPools;
    public List<GameObject>[] NormalUnitPool { get { return _normalUnitPools; } set { _normalUnitPools = value; } }


    private List<GameObject>[] _rareUnitPools;
    public List<GameObject>[] RareUnitPool { get { return _rareUnitPools; } set { _rareUnitPools = value; } }


    private List<GameObject>[] _ancientUnitPools;
    public List<GameObject>[] AncientUnitPool { get { return _ancientUnitPools; } set { _ancientUnitPools = value; } }


    private List<GameObject>[] _legendUnitPools;
    public List<GameObject>[] LegendUnitPool { get { return _legendUnitPools; } set { _legendUnitPools = value; } }


    private List<GameObject>[] _epicUnitPools;
    public List<GameObject>[] EpicUnitPool { get { return _epicUnitPools; } set { _epicUnitPools = value; } }


    private List<GameObject>[] _godUnitPools;
    public List<GameObject>[] GodUnitPool { get { return _godUnitPools; } set { _godUnitPools = value; } }

}
