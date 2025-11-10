using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MobData")]
public class MobData : ScriptableObject
{
    [SerializeField] private int _maxHp;
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
   

    [SerializeField] private string _name;
    public string Name { get { return _name; } set { _name = value; } }

    [SerializeField] private int _jemForKill;

    public int JemForKill { get { return _jemForKill; } set { _jemForKill = value; } }


}
