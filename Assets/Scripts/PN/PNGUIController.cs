using System.Collections;
using TMPro;
using UnityEngine;

public class PNGUIController : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI promptsText;

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
        coinsText.text = PlayerPrefs.GetInt("HuntedPrompts").ToString();
    }
}
