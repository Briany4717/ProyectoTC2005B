using UnityEngine;


/// Controla el movimiento vertical del enemigo para seguir la posición de las plataformas o del jugador.

public class HOEnemyVerticalFollow : MonoBehaviour, IHOScalableEnemy, IHOEnemyReward
{
    public int coinsBase = 3;
    public int incrementoCoins = 1;
    public int coinsMaximo = 15;

    public float timeBase = 2f;
    public float incrementoTime = 0.4f;
    public float timeMaximo = 8f;
    public int danioBase = 1;
    public int incrementoDanio = 1;
    public int danioMaximo = 4;

    public float velocidadBalaBase = 5f;
    public float incrementoVelocidadBala = 20f;
    public float velocidadBalaMaxima = 100f;

    [HideInInspector] public int danioActual;
    [HideInInspector] public float velocidadBalaActual;

    public Transform player;
    public float posFijaX = -8f;
    public LayerMask platformLayer;
    public float raycastDist = 20f;
    public float velocidad = 10f;
    public float timeEntreSalto = 0.2f;

    private float targetY;
    private float lastRepositionTime;
    private bool isEntering = true;

    public bool IsEntering {get {return isEntering;}}

    private int coinsActuales;
    private float timeActual;

    
    /// Inicializa las estadísticas actuales base.
    
    void Awake()
    {
        danioActual = danioBase;
        velocidadBalaActual = velocidadBalaBase;
        coinsActuales = coinsBase;
        timeActual = timeBase;
    }

    
    /// Busca al jugador si no está asignado y establece la posición objetivo inicial.
    
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

    
    /// Maneja el estado de entrada y actualiza el objetivo de movimiento.
    
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

    
    /// Actualiza la altura objetivo proyectando un rayo hacia abajo.
    
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

    
    /// Mueve al enemigo suavemente hacia la altura objetivo.
    
    void MoveToTarget()
    {
        Vector3 cntPos = transform.position;
        float newY;

        newY = Mathf.MoveTowards(cntPos.y, targetY, velocidad * Time.deltaTime);
        
        transform.position = new Vector3(posFijaX, newY, cntPos.z);
    }

    
    /// Ajusta la dificultad escalando el daño, velocidad y recompensas.
    
    public void SetDifficulty(int level)
    {
        danioActual = Mathf.Min(danioBase + level * incrementoDanio, danioMaximo);
        velocidadBalaActual = Mathf.Min(velocidadBalaBase + level * incrementoVelocidadBala, velocidadBalaMaxima);
        coinsActuales = Mathf.Min(coinsBase + level * incrementoCoins, coinsMaximo);
        timeActual = Mathf.Min(timeBase + level * incrementoTime, timeMaximo);
    }

    
    /// Mueve al enemigo a la posición inicial en pantalla.
    
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
    
    
    /// Devuelve la cantidad de monedas como recompensa.
    
    public int GetCoinsReward()
    {
        return coinsActuales;
    }

    
    /// Devuelve el tiempo extra como recompensa.
    
    public float GetTimeReward()
    {
        return timeActual;
    }
}