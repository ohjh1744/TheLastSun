using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Map : MonoBehaviour
{
    [SerializeField] AssetReferenceSprite[] _palletSprites;

    private Sprite plateSprite;
    private SpriteRenderer _plateRenderer;

    private void Start()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("Map: 자식 오브젝트가 없습니다. 첫 번째 자식(Plate)에 SpriteRenderer가 필요합니다.");
            return;
        }

        var plate = transform.GetChild(0);
        _plateRenderer = plate.GetComponent<SpriteRenderer>();
        if (_plateRenderer == null)
        {
            Debug.LogError("Map: 첫 번째 자식(Plate)에 SpriteRenderer 컴포넌트가 없습니다.");
            return;
        }

        plateSprite = _plateRenderer.sprite;

        var pc = PlayerController.Instance;
        int stage = (pc != null && pc.PlayerData != null) ? pc.PlayerData.CurrentStage : 0;
        ApplyPlateImage(stage);
    }

    public void ApplyPlateImage(int curStage)
    {
        if (_palletSprites == null || _palletSprites.Length == 0)
        {
            Debug.LogWarning("Map: _palletSprites가 비어있습니다.");
            return;
        }
        if (curStage < 0 || curStage >= _palletSprites.Length)
        {
            Debug.LogWarning($"Map: 잘못된 스테이지 인덱스 {curStage}");
            return;
        }

        AddressableManager.Instance.LoadOnlySprite(_palletSprites[curStage], sprite =>
        {
            plateSprite = sprite;
            if (_plateRenderer != null)
            {
                _plateRenderer.sprite = sprite; // 실제 Plate의 SpriteRenderer에 적용
            }
        });
    }
}
