using UnityEngine;
using TMPro;

public class HOUICoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    public string prefix = "";
    public string suffix = "";

    void Start()
    {
        if (coinsText == null)
        {
            Debug.LogError("HOUICoins: no se asignó el TextMeshProUGUI.");
            enabled = false;
            return;
        }

        if (HOCoins.Instance == null)
        {
            Debug.LogError("HOUICoins: no se encontró HOCoins en la escena.");
            enabled = false;
            return;
        }

        // Suscríbete al evento para actualizar la UI cuando cambien las monedas
        HOCoins.Instance.OnCoinsChanged += UpdateCoins;

        // Actualiza la UI con el valor inicial
        UpdateCoins(HOCoins.Instance.CurrentCoins);
    }

    void OnDestroy()
    {
        // Desuscribirse al destruirse para evitar errores
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.OnCoinsChanged -= UpdateCoins;
        }
    }

    void UpdateCoins(int newAmount)
    {
        coinsText.text = prefix + newAmount.ToString() + suffix;
    }
}