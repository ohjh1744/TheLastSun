using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EHeroPool {N_Warrior, R_Warrior, A_Warrior, L_Warrior, E_Warrior, 
    N_Archer, R_Archer, A_Archer, L_Archer, E_Archer, 
    N_Bomer, R_Bomer, A_Bomer, L_Bomer, E_Bomer, 
    God_1, God_2, God_3};
public enum EHeroGrade { Normal, Rare, Ancient, Legend, Epic, God };

public enum EProjectilePool
{
    E_Warrior,
    N_Archer, R_Archer, A_Archer, L_Archer, E_Archer,
    N_Bomer, R_Bomer, A_Bomer, L_Bomer, E_Bomer,
    God_1, God_2, God_3, Null
};

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance { get { return _instance; } private set { } }

    // 소환할 Origin Object
    List<GameObject> _mobs  = new List<GameObject>(); public List<GameObject> Mobs { get { return _mobs; } set { _mobs = value; } }
    List<GameObject> _heros = new List<GameObject>(); public List<GameObject> Heros { get { return _heros; } set { _heros = value; } }
    List<GameObject> _projectiles = new List<GameObject>(); public List<GameObject> Projectiles { get { return _projectiles; } set { _projectiles = value; } }

    // Pool
    private List<GameObject>[] _heroPools = new List<GameObject>[18]; public List<GameObject>[] HeroPools { get { return _heroPools; } set { _heroPools = value; } }
    private List<GameObject>[] _mobPools = new List<GameObject>[50]; public List<GameObject>[] MobPools { get { return _mobPools; } set { _mobPools = value; } }
    private List<GameObject>[] _projectilePools = new List<GameObject>[14]; public List<GameObject>[] ProjectilePools { get { return _projectilePools; } set { _projectilePools = value; } }

    //영웅 및 몹 현재 수
    private int _mobNum; public int MobNum { get { return _mobNum; } set { _mobNum = value; MobNumOnChanged?.Invoke(); } }
    public event UnityAction MobNumOnChanged;
    private int[] _heroNum = new int[18];

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        for (int i = 0; i < _heroPools.Length; i++)
        {
            _heroPools[i] = new List<GameObject>();
        }
        for (int i = 0; i < _mobPools.Length; i++)
        {
            _mobPools[i] = new List<GameObject>();
        }
        for (int i = 0; i < _projectilePools.Length; i++)
        {
            _projectilePools[i] = new List<GameObject>();
        }
    }
    public GameObject GetObject(List<GameObject>[] pools, List<GameObject> spawnobject, int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(spawnobject[index], transform);
            select.SetActive(true);
            pools[index].Add(select);
        }

        return select;
    }

    public void RemoveObject(List<GameObject>[] pools, int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (item.activeSelf)
            {
                select = item;
                select.SetActive(false);
                break;
            }
        }
    }

    //에픽까지
    public event UnityAction<int> HeroNumOnChanged;

    public int GetHeroNum(int heroIndex)
    {
        return _heroNum[heroIndex];
    }
    public void SetHeroNum(int heroIndex, int newNum)
    {
        _heroNum[heroIndex] = newNum;
        HeroNumOnChanged?.Invoke(heroIndex);
    }

}
