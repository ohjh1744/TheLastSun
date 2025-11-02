using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaterialLoader : MonoBehaviour
{
    private static Material _defaultMat;
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 한 번만 생성해서 모든 오브젝트가 공유
        if (_defaultMat == null)
        {
            Debug.Log("한번만 생성");
            _defaultMat = new Material(Shader.Find("Sprites/Default"));
        }

        sr.sharedMaterial = _defaultMat;
        Debug.Log(sr.sharedMaterial);
    }
}
