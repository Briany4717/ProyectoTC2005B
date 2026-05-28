using UnityEngine;
using UnityEngine.UI;


/// Controla el comportamiento visual e interactivo de un botón de pausa.

public class BotonPausaUI : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spritePlay;
    public Sprite spritePausa;

    private Image imagenBoton;
    private bool  pausado = false;

    
    /// Obtiene la referencia de la imagen del botón al despertar.
    
    void Awake()
    {
        imagenBoton = GetComponent<Image>();
    }

    
    /// Inicializa el estado visual del botón.
    
    void Start()
    {
        ResetearSprite();
    }

    
    /// Cambia entre el estado de juego y pausa cuando se hace clic en el botón.
    
    public void AlHacerClick()
    {
        pausado = !pausado;

        if (pausado)
        {
            imagenBoton.sprite = spritePlay;
            MenuPausa.instancia.Pausar();
        }
        else
        {
            imagenBoton.sprite = spritePausa;
            MenuPausa.instancia.Reanudar();
        }
    }

    
    /// Restablece el botón visualmente a su estado de jugar (no pausado).
    
    public void ResetearSprite()
    {
        pausado = false;
        if (imagenBoton != null && spritePausa != null)
            imagenBoton.sprite = spritePausa;
    }
}