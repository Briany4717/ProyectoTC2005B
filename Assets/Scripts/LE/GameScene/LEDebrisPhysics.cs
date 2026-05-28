using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LEDebrisPhysics : MonoBehaviour
{
    private Vector3 velocity;
    private float gravity = 1500f; // Pixeles por segundo al cuadrado
    private RectTransform rectTransform;
    private float screenBottomLimit = -700f; // Límite inferior de descarte en el Canvas

    public void InitializeDebris(Sprite sprite, Vector3 spawnPosition)
    {
        rectTransform = GetComponent<RectTransform>();
        GetComponent<Image>().sprite = sprite;
        
        transform.position = spawnPosition;
        rectTransform.localScale = Vector3.one;

        // Inyectamos fuerzas iniciales aleatorias (Arco hacia arriba y lados)
        float randomX = Random.Range(-350f, 350f);
        float randomY = Random.Range(400f, 850f);
        velocity = new Vector3(randomX, randomY, 0f);

        // Rotación aleatoria inicial para variedad estética
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }

    void Update()
    {
        // Aplicamos gravedad simulada al vector de velocidad (0% Físicas nativas de Unity)
        velocity.y -= gravity * Time.deltaTime;

        // Desplazamos el RectTransform en base a pixeles de UI
        rectTransform.anchoredPosition += (Vector2)(velocity * Time.deltaTime);

        // Auto-destrucción limpia al salir por el fondo del Canvas
        if (rectTransform.anchoredPosition.y < screenBottomLimit)
        {
            Destroy(gameObject);
        }
    }
}
