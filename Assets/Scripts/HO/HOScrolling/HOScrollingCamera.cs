using UnityEngine;

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

    public void ResetSpeed()
    {
        elapsedTime = 0f;
        currentSpeed = inicialSpeed;
    }
}
