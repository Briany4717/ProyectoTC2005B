using UnityEngine;
using UnityEngine.UI;


/// Añade automáticamente el sonido de clic de la UI a un botón.

[RequireComponent(typeof(Button))]
public class BotonSonido : MonoBehaviour
{
    [Tooltip("Desactiva el sonido en este botón específico")]
    public bool silenciado = false;

    
    /// Registra el evento de clic para reproducir el efecto de sonido si no está silenciado.
    
    void Start()
    {
        if (silenciado) return;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MusicController.instancia != null)
                MusicController.instancia.PlayClick();
        });
    }
}