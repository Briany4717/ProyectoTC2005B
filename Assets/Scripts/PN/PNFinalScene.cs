using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PNFinalScene : MonoBehaviour
{

    public TextMeshProUGUI coins, prompts;
    public Image background;
    public Sprite winSprite, loseSprite;

    void Update()
    {
    }

    void Start()
    {
        if (coins != null) coins.text = PlayerPrefs.GetInt("Coins", 0).ToString();
        if (prompts != null) prompts.text = PlayerPrefs.GetInt("HuntedPrompts", 0).ToString();
        
        int winStatus = PlayerPrefs.GetInt("Win", 0);

        if (PNSFXController.Instance != null)
        {
            PNSFXController.Instance.StopMusic();
        }

        if (winStatus == 1)
        {
            if (background != null && winSprite != null) background.sprite = winSprite;
            if (PNSFXController.Instance != null) PNSFXController.Instance.PlayWinMusic();
        }
        else
        {
            if (background != null && loseSprite != null) background.sprite = loseSprite;
            if (PNSFXController.Instance != null) PNSFXController.Instance.PlayLoseMusic();
        }

        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("HuntedPrompts", 0);
    }

}
