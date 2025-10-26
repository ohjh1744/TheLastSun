using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] AssetReferenceSprite[] _palletSprites;

    private Sprite plateSprite;

    private void Awake()
    {
        plateSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    private void Start()
    {
        ApplyPlateImage(PlayerController.Instance.PlayerData.CurrentStage);
    }

    public void ApplyPlateImage(int curStage)
    {
        AddressableManager.Instance.LoadOnlySprite(_palletSprites[curStage], (sprite)=> { plateSprite = sprite; });
    }
}
