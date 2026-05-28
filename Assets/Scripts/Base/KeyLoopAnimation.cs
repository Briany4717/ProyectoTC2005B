using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


/// Controla la animación de un botón o tecla alternando entre dos sprites e interactuando con su texto.

public class KeyLoopAnimation : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public float speed = 0.5f;

    private Image imageComponent;

    [SerializeField]
    private TextMeshProUGUI textComponent;

    private Color originalTextColor;
    private Vector3 originalTextScale;

    
    /// Inicializa los componentes, guarda los valores originales e inicia la corrutina de animación.
    
    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        }

        originalTextColor = textComponent.color;
        originalTextScale = textComponent.transform.localScale;

        StartCoroutine(AnimateKey());
    }

    
    /// Corrutina que alterna cíclicamente los estados visuales del botón y del texto.
    
    IEnumerator AnimateKey()
    {
        while (true)
        {
            imageComponent.sprite = pressedSprite;
            textComponent.color = Color.gray;

            textComponent.transform.localScale = originalTextScale * 0.9f;

            yield return new WaitForSeconds(speed);

            imageComponent.sprite = normalSprite;
            textComponent.color = originalTextColor;

            textComponent.transform.localScale = originalTextScale;

            yield return new WaitForSeconds(speed);
        }
    }
}
