using UnityEngine;

/// <summary>
/// Controla el desplazamiento vertical automático y acelerado de la cámara.
/// </summary>
public class HOScrollingCamera : MonoBehaviour
{
    public static HOScrollingCamera Instance { get; private set; }

    public float inicialSpeed;
    public float aceleracion;
    public float maxSpeed;

    private bool isScrolling = true;
    private Camera cam;
    private float currentSpeed;
    private float elapsedTime;

    public float bottomEdge {get {return transform.position.y - cam.orthographicSize;}}
    public float topEdge {get { return transform.position.y + cam.orthographicSize;}}
    public float CurrentSpeed {get{return currentSpeed;}}

    /// <summary>
    /// Inicializa el Singleton y obtiene la referencia de la cámara.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// Aumenta la velocidad gradualmente y mueve la cámara hacia arriba.
    /// </summary>
    void Update()
    {
        if (!isScrolling) 
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        currentSpeed = Mathf.Min(inicialSpeed + elapsedTime * aceleracion, maxSpeed);

        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Reinicia la velocidad de desplazamiento a su valor inicial.
    /// </summary>
    public void ResetSpeed()
    {
        elapsedTime = 0f;
        currentSpeed = inicialSpeed;
    }
    
    /// <summary>
    /// Reduce la dificultad bajando el tiempo transcurrido, disminuyendo así la velocidad.
    /// </summary>
    public void ReduceDifficulty(float percent)
    {
        percent = Mathf.Clamp01(percent);
        elapsedTime *= (1f - percent);
    }
}