using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona la interfaz de usuario para mostrar la cantidad de monedas.
/// </summary>
public class HOUICoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    public string prefix = "";
    public string suffix = "";

    /// <summary>
    /// Inicializa la UI y se suscribe a los cambios de monedas.
    /// </summary>
    void Start()
    {
        HOCoins.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(HOCoins.Instance.CurrentCoins);
    }

    /// <summary>
    /// Se desuscribe de los eventos al destruirse el objeto.
    /// </summary>
    void OnDestroy()
    {
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.OnCoinsChanged -= UpdateCoins;
        }
    }

    /// <summary>
    /// Actualiza el texto de las monedas en la interfaz.
    /// </summary>
    void UpdateCoins(int newAmount)
    {
        coinsText.text = prefix + newAmount.ToString() + suffix;
    }
}