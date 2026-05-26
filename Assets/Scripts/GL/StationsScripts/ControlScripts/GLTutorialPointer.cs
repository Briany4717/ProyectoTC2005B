using UnityEngine;

public class GLTutorialPointer : MonoBehaviour
{
    [Header("Referencias de Posición")]
    public Transform startPoint;

    public Transform endPoint;

    [Header("Ajustes de Animación")]
    public float speed = 2f;

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        // Mathf.PingPong crea un valor que sube y baja entre 0 y 1 repetidamente.
        // Al usar % 1 (módulo), hacemos que vaya de 0 a 1 y luego se reinicie de golpe,
        // perfecto para un tutorial de arrastre que se repite.
        float t = (Time.time * speed) % 1f;

        // Movemos el puntero interpolando entre el inicio y el final
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        // Desvanecer el cursor cuando se acerca al final
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            color.a = 1f - t; // Transparente al llegar al punto final
            sr.color = color;
        }
    }
}
