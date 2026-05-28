using UnityEngine;
using TMPro;


/// Muestra las monedas obtenidas en la pantalla de fin de juego.

public class HOEndSceneCoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    
    /// Carga las monedas guardadas y las muestra en la interfaz.
    
    void Start()
    {
        int finalCoins = PlayerPrefs.GetInt("LastGameCoins", 0);
        coinsText.text = finalCoins.ToString();
    }
}