using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class TTestPanel : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AssetReferenceGameObject _object;
    void Start()
    {
        AddressableManager.Instance.DoCheckDownLoadFile((size) => { Debug.Log($"{size}"); });
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            SceneManager.LoadScene(1);
        }
    }

}
