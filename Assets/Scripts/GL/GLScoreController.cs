using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GLScoreController : MonoBehaviour
{

    public static GLScoreController Instance;
    int coins = 0;
    int totalOrders = 0;


    public TextMeshProUGUI coinsText;


    private void Awake()
    {
        Instance = this;
        PlayerPrefs.SetInt("Orders", 0);
        PlayerPrefs.SetInt("Coins", 0);
    }

    void Start()
    {
        coinsText.text = coins.ToString();
    }

    public void AddOrderCoins(int points)
    {
        coins += points;
        coinsText.text = coins.ToString();
        totalOrders++;

        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Orders", totalOrders);
    }
}
