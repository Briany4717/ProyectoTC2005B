using System.Collections;
using TMPro;
using UnityEngine;

public class PNGUIController : MonoBehaviour
{
    public TextMeshProUGUI coinsText, promptsText;
    public SpriteRenderer PNSky; 
    public Sprite[] skySprites; 
    public static PNGUIController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coinsText.text = "0";
        promptsText.text = "0";
    }

    public void setCoin()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins").ToString();
    }

    public void setPrompt()
    {
        promptsText.text = PlayerPrefs.GetInt("HuntedPrompts").ToString();
    }

    public void ChangeSkyAsset(int spriteIndex)
    {
        if (PNSky == null) return;

        if (skySprites != null && spriteIndex >= 0 && spriteIndex < skySprites.Length)
        {
            PNSky.sprite = skySprites[spriteIndex];
        }
    }
}