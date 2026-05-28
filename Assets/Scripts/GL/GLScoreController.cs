using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

/// <summary>
/// Controla la puntuación y el registro de órdenes completadas.
/// </summary>
public class GLScoreController : MonoBehaviour
{
    public static GLScoreController Instance;
    int coins = 0;
    int totalOrders = 0;

    public TextMeshProUGUI coinsText;

    /// <summary>
    /// Configura el Singleton y reinicia los valores guardados de puntuación.
    /// </summary>
    private void Awake()
    {
        Instance = this;
        PlayerPrefs.SetInt("Orders", 0);
        PlayerPrefs.SetInt("Coins", 0);
    }

    /// <summary>
    /// Inicializa la visualización de monedas en pantalla.
    /// </summary>
    void Start()
    {
        coinsText.text = coins.ToString();
    }

    /// <summary>
    /// Agrega monedas por órdenes entregadas y actualiza el progreso guardado.
    /// </summary>
    public void AddOrderCoins(int points)
    {
        coins += points;
        coinsText.text = coins.ToString();
        totalOrders++;

        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Orders", totalOrders);
    }
}