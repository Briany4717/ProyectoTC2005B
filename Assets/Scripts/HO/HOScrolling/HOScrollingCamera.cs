using UnityEngine;

public class HOScrollingCamera : MonoBehaviour
{
    public static HOScrollingCamera Instance { get; private set; }

    [Header("Velocidad de scroll")]
    public float baseSpeed = 1f;
    public float accelerationRate = 0.05f;
    public float maxSpeed = 5f;

    public bool isScrolling = true;

    private Camera cam;
    private float currentSpeed;
    private float elapsedTime;

    // Propiedades públicas para que otros scripts conozcan los bordes de la cámara
    public float BottomEdgeY => transform.position.y - cam.orthographicSize;
    public float TopEdgeY => transform.position.y + cam.orthographicSize;
    public float CurrentSpeed => currentSpeed;

    void Awake()
    {
        // Patrón singleton: garantiza una única instancia accesible globalmente
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cam = GetComponent<Camera>();

        // La cámara debe ser ortográfica para un juego 2D
        if (!cam.orthographic)
        {
            Debug.LogWarning("HOScrollingCamera: la cámara no es ortográfica. Se recomienda usar modo ortográfico para 2D.");
        }
    }

    void Update()
    {
        if (!isScrolling) return;

        // Calcula la velocidad actual: base + (tiempo * aceleración), limitada por maxSpeed
        elapsedTime += Time.deltaTime;
        currentSpeed = Mathf.Min(baseSpeed + elapsedTime * accelerationRate, maxSpeed);

        // Mueve la cámara hacia arriba
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Reinicia el contador de tiempo y vuelve a la velocidad base.
    /// Útil al reiniciar la partida.
    /// </summary>
    public void ResetSpeed()
    {
        elapsedTime = 0f;
        currentSpeed = baseSpeed;
    }

    void OnDrawGizmos()
    {
        // Visualiza los bordes superior e inferior en el editor
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic) return;

        Gizmos.color = Color.green;
        float halfWidth = cam.orthographicSize * cam.aspect;
        float topY = transform.position.y + cam.orthographicSize;
        float bottomY = transform.position.y - cam.orthographicSize;

        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, topY, 0), new Vector3(transform.position.x + halfWidth, topY, 0));
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, bottomY, 0), new Vector3(transform.position.x + halfWidth, bottomY, 0));
    }
}
