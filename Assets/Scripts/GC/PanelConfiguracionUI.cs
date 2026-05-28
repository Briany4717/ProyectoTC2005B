using UnityEngine;
using UnityEngine.UI;

public class PanelConfiguracionUI : MonoBehaviour
{
    public Slider sliderMusica;
    public Slider sliderSonido;

    void OnEnable()
    {
        // Cargar valores guardados sin disparar eventos
        if (sliderMusica != null)
        {
            sliderMusica.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("VolMusica", 0.5f));

            // Registrar listener en código → siempre usa MusicController.instancia
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

    void OnDisable()
    {
        // Limpiar listeners al cerrar el panel
        if (sliderMusica != null) sliderMusica.onValueChanged.RemoveAllListeners();
        if (sliderSonido != null) sliderSonido.onValueChanged.RemoveAllListeners();
    }

    private void OnMusicaChanged(float valor) =>
        MusicController.instancia?.SetVolumenMusica(valor);

    private void OnSonidoChanged(float valor) =>
        MusicController.instancia?.SetVolumenSFX(valor);
}