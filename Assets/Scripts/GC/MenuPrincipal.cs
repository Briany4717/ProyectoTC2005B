using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelInstrucciones;
    public GameObject panelConfiguracion;

    [Header("Configuración — Audio")]
    public Slider sliderMusica;
    public Slider sliderSonido;

    void Start()
    {
        panelMenu.SetActive(true);
        panelInstrucciones.SetActive(false);
        panelConfiguracion.SetActive(false);

        // SetValueWithoutNotify carga el valor SIN disparar OnValueChanged
        if (sliderMusica != null)
            sliderMusica.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolMusica", 0.5f));
        if (sliderSonido != null)
            sliderSonido.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolSFX", 1f));
    }

    // ── Navegación ────────────────────────────────────────────────────

    public void NuevaPartida()
    {
        MusicController.instancia?.ReiniciarMusica();
        SceneManager.LoadScene("GCInicial");
    }

    public void AbrirInstrucciones()
    {
        panelMenu.SetActive(false);
        panelInstrucciones.SetActive(true);
    }

    public void AbrirConfiguracion()
    {
        panelMenu.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void VolverAlMenu()
    {
        panelInstrucciones.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelMenu.SetActive(true);
    }

    // ── Audio (ruteado por MusicController, claves unificadas) ────────

    public void CambiarMusica(float valor)
    {
        MusicController.instancia?.SetVolumenMusica(valor);
    }

    public void CambiarSonido(float valor)
    {
        MusicController.instancia?.SetVolumenSFX(valor);
    }
}