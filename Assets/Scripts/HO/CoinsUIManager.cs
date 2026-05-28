using UnityEngine;
using TMPro;


/// Gestiona la interfaz de usuario para mostrar la cantidad de monedas.

public class HOUICoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    public string prefix = "";
    public string suffix = "";

    
    /// Inicializa la UI y se suscribe a los cambios de monedas.
    
    void Start()
    {
        HOCoins.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(HOCoins.Instance.CurrentCoins);
    }

    
    /// Se desuscribe de los eventos al destruirse el objeto.
    
    void OnDestroy()
    {
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.OnCoinsChanged -= UpdateCoins;
        }
    }

    
    /// Actualiza el texto de las monedas en la interfaz.
    
    void UpdateCoins(int newAmount)
    {
        coinsText.text = prefix + newAmount.ToString() + suffix;
    }
}