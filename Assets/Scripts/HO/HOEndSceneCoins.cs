using UnityEngine;
using TMPro;

/// <summary>
/// Muestra las monedas obtenidas en la pantalla de fin de juego.
/// </summary>
public class HOEndSceneCoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    /// <summary>
    /// Carga las monedas guardadas y las muestra en la interfaz.
    /// </summary>
    void Start()
    {
        int finalCoins = PlayerPrefs.GetInt("LastGameCoins", 0);
        coinsText.text = finalCoins.ToString();
    }
}