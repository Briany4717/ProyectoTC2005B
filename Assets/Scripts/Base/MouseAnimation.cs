using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// Controla la animación de un botón de ratón alternando entre dos sprites.

public class MouseAnimation : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public float speed = 0.5f;

    private Image imageComponent;

    
    /// Inicializa el componente de imagen y comienza la corrutina de animación.
    
    void Start()
    {
        imageComponent = GetComponent<Image>();
        StartCoroutine(AnimateKey());
    }

    
    /// Corrutina que alterna continuamente los sprites para simular clics.
    
    IEnumerator AnimateKey()
    {
        while (true)
        {
            imageComponent.sprite = pressedSprite;
            yield return new WaitForSeconds(speed);

            imageComponent.sprite = normalSprite;
            yield return new WaitForSeconds(speed);
        }
    }
}
