using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controla la animación de un botón de ratón alternando entre dos sprites.
/// </summary>
public class MouseAnimation : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public float speed = 0.5f;

    private Image imageComponent;

    /// <summary>
    /// Inicializa el componente de imagen y comienza la corrutina de animación.
    /// </summary>
    void Start()
    {
        imageComponent = GetComponent<Image>();
        StartCoroutine(AnimateKey());
    }

    /// <summary>
    /// Corrutina que alterna continuamente los sprites para simular clics.
    /// </summary>
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
