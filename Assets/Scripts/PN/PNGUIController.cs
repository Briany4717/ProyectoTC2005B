using UnityEngine;

public class PNGUIController : MonoBehaviour
{
    [Header("Sky backgrounds: 0=Prompt, 1=Golden, 2=Storm")]
    public SpriteRenderer skyRenderer;
    public Sprite[] skyAssets;

    public void ChangeSkyAsset(int state)
    {
        if (skyRenderer == null) return;
        if (skyAssets == null || state < 0 || state >= skyAssets.Length) return;
        skyRenderer.sprite = skyAssets[state];
    }
}
