using UnityEngine;
using UnityEngine.UI;


/// Controla la UI del panel de configuración dentro del juego.

public class PanelConfiguracionUI : MonoBehaviour
{
    public Slider sliderMusica;
    public Slider sliderSonido;

    
    /// Carga los valores de configuración guardados al activar el panel.
    
    void OnEnable()
    {
        if (sliderMusica != null)
        {
            sliderMusica.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("VolMusica", 0.5f));

            sliderMusica.onValueChanged.RemoveAllListeners();
            sliderMusica.onValueChanged.AddListener(OnMusicaChanged);
        }

        if (sliderSonido != null)
        {
            sliderSonido.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("VolSFX", 1f));

            sliderSonido.onValueChanged.RemoveAllListeners();
            sliderSonido.onValueChanged.AddListener(OnSonidoChanged);
        }
    }

    
    /// Limpia los escuchadores de eventos al desactivar el panel.
    
    void OnDisable()
    {
        if (sliderMusica != null) sliderMusica.onValueChanged.RemoveAllListeners();
        if (sliderSonido != null) sliderSonido.onValueChanged.RemoveAllListeners();
    }

    
    /// Cambia el volumen de la música.
    
    private void OnMusicaChanged(float valor) =>
        MusicController.instancia?.SetVolumenMusica(valor);

    
    /// Cambia el volumen de los efectos de sonido.
    
    private void OnSonidoChanged(float valor) =>
        MusicController.instancia?.SetVolumenSFX(valor);
}