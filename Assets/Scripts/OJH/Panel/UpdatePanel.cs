using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePanel : MonoBehaviour
{
    [SerializeField] private GameObject _downPanel;
    void Start()
    {
        Debug.Log(GpgsManager.Instance);
        GpgsManager.Instance.DoCheckForUpdate(_downPanel);
    }
 
}
