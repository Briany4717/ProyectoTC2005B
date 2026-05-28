using UnityEngine;
using TMPro;


/// Controla la cantidad de rayos o energía disponible del jugador.

public class SistemaRayos : MonoBehaviour
{
    [Header("Rayos")]
    public int rayosMaximos = 5;
    public int rayosActuales = 0;

    [Header("UI")]
    public TMP_Text textoContador;
    public TMP_Text textoContadorCamaras;
    public TMP_Text textoContadorHUD;

    public static SistemaRayos instancia;

    
    /// Configura la instancia Singleton.
    
    void Awake() => instancia = this;

    
    /// Muestra la cantidad inicial de rayos en la interfaz.
    
    void Start() => ActualizarUI();

    
    /// Incrementa la cantidad de rayos si no se ha alcanzado el límite máximo.
    
    public void GanarRayo()
    {
        if (rayosActuales < rayosMaximos)
        {
            rayosActuales++;
            ActualizarUI();
        }
    }

    
    /// Disminuye la cantidad de rayos por fallar alguna acción.
    
    public void PerderRayo()
    {
        if (rayosActuales > 0)
        {
            rayosActuales--;
            ActualizarUI();
        }
    }

    
    /// Consume un rayo para realizar una acción, si hay disponibles.
    
    public bool UsarRayo()
    {
        if (rayosActuales > 0)
        {
            rayosActuales--;
            ActualizarUI();
            return true;
        }
        Debug.Log("Sin rayos disponibles");
        return false;
    }

    
    /// Refleja la cantidad de rayos actuales en todos los textos de UI correspondientes.
    
    void ActualizarUI()
    {
        if (textoContador != null)
            textoContador.text = $"x{rayosActuales}";
        if (textoContadorCamaras != null)
            textoContadorCamaras.text = $"x{rayosActuales}";
        if (textoContadorHUD != null)
            textoContadorHUD.text = $"x{rayosActuales}";
    }
}