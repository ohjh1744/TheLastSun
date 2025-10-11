using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TTestPanel : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AssetReferenceGameObject _object;
    void Start()
    {
        AddressableManager.Instance.GetObject(_object, (obj) => { });
    }

}
