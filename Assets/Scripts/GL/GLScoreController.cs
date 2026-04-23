using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GLScoreController : MonoBehaviour
{

    public static GLScoreController Instance;

    public Text coinsText;

    int coins = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coinsText.text = coins.ToString();
    }

    public void AddPoints(int points)
    {
        coins += points;
        coinsText.text = coins.ToString();
    }
}
