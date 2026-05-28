using UnityEngine;
using TMPro;


/// Actualiza el texto de las monedas en el menú principal.

public class MonedasMenuPrincipal : MonoBehaviour
{
    public TMP_Text textoMonedas;

    
    /// Actualiza la visualización de monedas al iniciar.
    
    void Start() => ActualizarMonedas();

    
    /// Actualiza la visualización de monedas al activar el objeto.
    
    void OnEnable() => ActualizarMonedas();

    
    /// Obtiene y muestra el total de monedas guardadas.
    
    void ActualizarMonedas()
    {
        int total = PlayerPrefs.GetInt("MonedasTotal", 0);
        if (textoMonedas != null)
            textoMonedas.text = $"{total}";
    }
}