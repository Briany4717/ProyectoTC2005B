using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


/// Controla la puntuación y el registro de órdenes completadas.

public class GLScoreController : MonoBehaviour
{
    public static GLScoreController Instance;
    int coins = 0;
    int totalOrders = 0;

    public TextMeshProUGUI coinsText;

    
    /// Configura el Singleton y reinicia los valores guardados de puntuación.
    
    private void Awake()
    {
        Instance = this;
        PlayerPrefs.SetInt("Orders", 0);
        PlayerPrefs.SetInt("Coins", 0);
    }

    
    /// Inicializa la visualización de monedas en pantalla.
    
    void Start()
    {
        coinsText.text = coins.ToString();
    }

    
    /// Agrega monedas por órdenes entregadas y actualiza el progreso guardado.
    
    public void AddOrderCoins(int points)
    {
        coins += points;
        coinsText.text = coins.ToString();
        totalOrders++;

        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Orders", totalOrders);
    }
}