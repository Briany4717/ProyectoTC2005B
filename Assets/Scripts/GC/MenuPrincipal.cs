using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// Controla la navegación del menú principal y configuraciones iniciales.

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelInstrucciones;
    public GameObject panelConfiguracion;

    [Header("Configuración — Audio")]
    public Slider sliderMusica;
    public Slider sliderSonido;

    
    /// Inicializa la visibilidad de los paneles y los valores de audio guardados.
    
    void Start()
    {
        panelMenu.SetActive(true);
        panelInstrucciones.SetActive(false);
        panelConfiguracion.SetActive(false);

        if (sliderMusica != null)
            sliderMusica.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolMusica", 0.5f));
        if (sliderSonido != null)
            sliderSonido.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolSFX", 1f));
    }

    
    /// Carga la escena principal del juego y reinicia la música.
    
    public void NuevaPartida()
    {
        MusicController.instancia?.ReiniciarMusica();
        SceneManager.LoadScene("GCInicial");
    }

    
    /// Muestra el panel de instrucciones ocultando el menú principal.
    
    public void AbrirInstrucciones()
    {
        panelMenu.SetActive(false);
        panelInstrucciones.SetActive(true);
    }

    
    /// Muestra el panel de configuración ocultando el menú principal.
    
    public void AbrirConfiguracion()
    {
        panelMenu.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    
    /// Cierra la aplicación por completo.
    
    public void SalirDelJuego()
    {
        Application.Quit();
    }

    
    /// Regresa al panel del menú principal.
    
    public void VolverAlMenu()
    {
        panelInstrucciones.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelMenu.SetActive(true);
    }

    
    /// Ajusta el volumen de la música.
    
    public void CambiarMusica(float valor)
    {
        MusicController.instancia?.SetVolumenMusica(valor);
    }

    
    /// Ajusta el volumen de los efectos de sonido.
    
    public void CambiarSonido(float valor)
    {
        MusicController.instancia?.SetVolumenSFX(valor);
    }
}