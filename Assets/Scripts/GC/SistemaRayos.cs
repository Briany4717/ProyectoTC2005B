using UnityEngine;
using TMPro;

public class SistemaRayos : MonoBehaviour
{
    [Header("Rayos")]
    public int rayosMaximos = 5;
    public int rayosActuales = 0;

    [Header("UI")]
    public TMP_Text textoContador;           // En PanelGenerador
    public TMP_Text textoContadorCamaras;    // En PanelVista
    public TMP_Text textoContadorHUD;        // En el juego principal (nuevo)

    public static SistemaRayos instancia;

    void Awake() => instancia = this;
    void Start() => ActualizarUI();

    public void GanarRayo()
    {
        if (rayosActuales < rayosMaximos)
        {
            rayosActuales++;
            ActualizarUI();
        }
    }

    public void PerderRayo()
    {
        if (rayosActuales > 0)
        {
            rayosActuales--;
            ActualizarUI();
        }
    }

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