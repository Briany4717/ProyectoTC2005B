using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseAnimation : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;
    public float speed = 0.5f; // Segundos entre cada cambio

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        // Iniciamos el ciclo infinito
        StartCoroutine(AnimateKey());
    }

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
