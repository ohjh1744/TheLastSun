using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EMaterial {Default, PF }
public class ObjectMaterialLoader : MonoBehaviour
{
    [SerializeField] EMaterial eMaterial;

    private static Material _defaultMat;
    private static Material _pfMat;

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

            // PF용 Shader 적용 (Distance Field 기반)
            if(_pfMat == null)
            {
                _pfMat = Resources.Load<Material>("PFStardust 3 Material");
                //이걸 Tmp Shader에 적용하기
            }

            tmp.fontMaterial = _pfMat;        // 인스턴스용
            tmp.fontSharedMaterial = _pfMat;  // 공유용

        }


    }
}
