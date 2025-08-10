using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePanel : MonoBehaviour
{
    void Start()
    {
        GpgsManager.Instance.DoCheckForUpdate(gameObject);
    }
 
}
