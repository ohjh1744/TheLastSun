using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPuller : MonoBehaviour
{

    //Object Pulling
    [SerializeField] private UnitPool _unitPool;
    public UnitPool UnitPool { get { return _unitPool; } set { _unitPool = value; } }

    [SerializeField] private MobPool _mobPool;
    public MobPool MobPool { get { return _mobPool; } set { _mobPool = value; } }

    public GameObject GetUnit(EUnit unitType, List<GameObject> SavedUnit, List<GameObject>[] unitPools)
    {
        GameObject select = null;

        int typeNum = (int)unitType;

        foreach (GameObject item in unitPools[typeNum])
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
            select = Instantiate(SavedUnit[typeNum], transform);
            unitPools[typeNum].Add(select);
        }

        return select;
    }

    public GameObject GetMob(int waveIndex, List<GameObject> SavedMob, List<GameObject>[] mobPools)
    {
        GameObject select = null;

        foreach (GameObject item in mobPools[waveIndex])
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
            select = Instantiate(SavedMob[waveIndex], transform);
            mobPools[waveIndex].Add(select);
        }
        return select;
    }



}
