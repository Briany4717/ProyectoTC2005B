using UnityEngine;
using UnityEngine.UI;

public class BotonPausaUI : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spritePlay;
    public Sprite spritePausa;

    private Image imagenBoton;
    private bool  pausado = false;

    void Awake()
    {
        imagenBoton = GetComponent<Image>();
    }

    void Start()
    {
        ResetearSprite();
    }

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

    public void ResetearSprite()
    {
        pausado = false;
        if (imagenBoton != null && spritePausa != null)
            imagenBoton.sprite = spritePausa;
    }
}