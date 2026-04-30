using UnityEngine;
using TMPro;

public class HOUICoins : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    public string prefix = "";
    public string suffix = "";

    void Start()
    {
        // suscribirse al evento
        HOCoins.Instance.OnCoinsChanged += UpdateCoins;

        // actualizar la UI 
        UpdateCoins(HOCoins.Instance.CurrentCoins);
    }

    void OnDestroy()
    {
        // desuscribirse al destruirse
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