using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class KeyLoopAnimation : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public float speed = 0.5f; // Segundos entre cada cambio

    private Image imageComponent;

    [SerializeField]
    private TextMeshProUGUI textComponent;

    private Color originalTextColor;
    private Vector3 originalTextScale; // Variable para guardar el tamaño original

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Guardamos el color y la escala con la que empieza el texto
        originalTextColor = textComponent.color;
        originalTextScale = textComponent.transform.localScale;

        StartCoroutine(AnimateKey());
    }

    IEnumerator AnimateKey()
    {
        while (true)
        {
            // ESTADO PRESIONADO 
            imageComponent.sprite = pressedSprite;
            textComponent.color = Color.gray;

            // Hacemos el texto un 10% más pequeño multiplicando su escala original por 0.9
            textComponent.transform.localScale = originalTextScale * 0.9f;

            yield return new WaitForSeconds(speed);

            // ESTADO NORMAL
            imageComponent.sprite = normalSprite;
            textComponent.color = originalTextColor;

            // Restauramos el tamaño exacto que tenía al inicio
            textComponent.transform.localScale = originalTextScale;

            yield return new WaitForSeconds(speed);
        }
    }
}