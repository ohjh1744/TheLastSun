using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum EPull { Unit, Mob };

public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private int _jemNum;
    public int JemNum { get { return _jemNum; } set { _jemNum = value; JemNumOnChanged?.Invoke(); } }

    public event UnityAction JemNumOnChanged;

    //게임 시작, 종료 관리
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
