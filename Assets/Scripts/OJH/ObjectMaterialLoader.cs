using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaterialLoader : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Default 매터리얼 가져오기
        Material defaultMat = new Material(Shader.Find("Sprites/Default"));

        // SpriteRenderer에 적용
        sr.material = defaultMat;

        Debug.Log(sr.material);
    }
}
