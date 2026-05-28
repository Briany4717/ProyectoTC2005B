using UnityEngine;
using TMPro;

public class MonedasMenuPrincipal : MonoBehaviour
{
    public TMP_Text textoMonedas; // TextoMonedas del menú principal

    void Start() => ActualizarMonedas();
    void OnEnable() => ActualizarMonedas();

    void ActualizarMonedas()
    {
        int total = PlayerPrefs.GetInt("MonedasTotal", 0);
        if (textoMonedas != null)
            textoMonedas.text = $"{total}";
    }
}