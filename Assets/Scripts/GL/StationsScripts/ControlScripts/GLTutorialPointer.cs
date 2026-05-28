using UnityEngine;


/// Controla la animación de un cursor de tutorial que se mueve entre dos puntos.

public class GLTutorialPointer : MonoBehaviour
{
    [Header("Referencias de Posición")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Ajustes de Animación")]
    public float speed = 2f;

    
    /// Actualiza la posición y transparencia del cursor interpolando entre los puntos de inicio y fin.
    
    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        float t = (Time.time * speed) % 1f;
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            color.a = 1f - t;
            sr.color = color;
        }
    }
}