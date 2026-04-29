using UnityEngine;

public class HOEnemyVerticalFollow : MonoBehaviour, IHOScalableEnemy
{
    public int danioBase = 1;
    public int incrementoDanio = 1;
    public int danioMaximo = 3;

    public float velocidadBalaBase = 5f;
    public float incrementoVelocidadBala = 2f;
    public float velocidadBalaMaxima = 15f;

    // HideInInspector porque solo los voy a acceder desde otro script
    [HideInInspector] public int danioActual;
    [HideInInspector] public float velocidadBalaActual;

    public Transform player;
    public float posFijaX = -8f;
    // Layermask para detectar plataformas
    public LayerMask platformLayer;
    public float raycastDist = 20f;
    public float velocidad = 10f;
    public float timeEntreSalto = 0.2f;

    private float targetY;
    private float lastRepositionTime;
    private bool isEntering = true;

    public bool IsEntering {get {return isEntering;}}


    void Awake()
    {
        danioActual = danioBase;
        velocidadBalaActual = velocidadBalaBase;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("HOPlayer");
            if (p != null) 
            {
                player = p.transform;
            }
        }

        targetY = transform.position.y;
    }

    void Update()
    {
        if (isEntering)
        {
            Entering();
            return;
        }
        if (player == null) 
        {
            return;
        }

        if (Time.time - lastRepositionTime >= timeEntreSalto)
        {
            UpdateTargetPlatform();
            lastRepositionTime = Time.time;
        }
        MoveToTarget();
    }

    void UpdateTargetPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, raycastDist, platformLayer);

        if (hit.collider != null)
        {
            targetY = hit.point.y;
        }
        else
        {
            targetY = player.position.y;
        }
    }

    void MoveToTarget()
    {
        Vector3 cntPos = transform.position;
        float newY;

        newY = Mathf.MoveTowards(cntPos.y, targetY, velocidad * Time.deltaTime);
        
        transform.position = new Vector3(posFijaX, newY, cntPos.z);
    }

    public void SetDifficulty(int level)
    {
        danioActual = Mathf.Min(danioBase + level * incrementoDanio, danioMaximo);
        velocidadBalaActual = Mathf.Min(velocidadBalaBase + level * incrementoVelocidadBala, velocidadBalaMaxima);
        Debug.Log($"Arma escalada: nivel {level}, daño={danioActual}, velocidad bala={velocidadBalaActual}");

    }

    void Entering()
    {
        float newX = Mathf.MoveTowards(transform.position.x, posFijaX, velocidad * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        if (Mathf.Abs(transform.position.x - posFijaX) < 0.05f)
        {
            transform.position = new Vector3(posFijaX, transform.position.y, transform.position.z);
            isEntering = false;
        }
    }

    /*
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position, player.position + Vector3.down * raycastDist);
        }
    }*/
}