using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPool : MonoBehaviour
{
    //어드레서블로 불러온 프리펩 저장하는 공간
    private List<GameObject> _savedMobs;
    public List<GameObject> SavedMobs { get { return _savedMobs; } set { _savedMobs = value; } }


    //게임 내에서 사용하는 실제 Pools
    private List<GameObject>[] _mobPools;
    public List<GameObject>[] MobPools { get { return _mobPools; } set { _mobPools = value; } }
}
