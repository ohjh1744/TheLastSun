using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EMaterial {Default, PF }
public class ObjectMaterialLoader : MonoBehaviour
{
    [SerializeField] EMaterial eMaterial;
    [SerializeField] Material _pf;

    private static Material _defaultMat;

    void Start()
    {
        if (eMaterial == EMaterial.Default)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            // 한 번만 생성해서 모든 오브젝트가 공유
            if (_defaultMat == null)
            {
                _defaultMat = new Material(Shader.Find("Sprites/Default"));
            }

            sr.sharedMaterial = _defaultMat;
        }
        else if ((eMaterial == EMaterial.PF))
        {
            TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
            tmp.fontMaterial = _pf;
        }


    }
}
