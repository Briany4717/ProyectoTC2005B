using UnityEngine;
using TMPro;

public class HOEndSceneCoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;


    void Start()
    {
        int finalCoins = PlayerPrefs.GetInt("LastGameCoins", 0);
        coinsText.text = finalCoins.ToString();
    }
}