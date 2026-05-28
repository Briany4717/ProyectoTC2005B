using System.Collections;
using UnityEngine;


/// Controla el comportamiento del enemigo borrador, su patrullaje y ataques a plataformas.

public class HOEraserEnemy : MonoBehaviour, IHOScalableEnemy, IHOEnemyReward
{
    public int coinsBase;
    public int incrementoCoins;
    public int coinsMaximo;

    public float timeBase = 3f;
    public float incrementoTime = 0.5f;
    public float timeMaximo = 10f;

    public float incrementoVelocidad = 1f;
    public float velocidadMaxima = 8f;
    public float decrementoIntervalo = 1f;
    public float intervaloMinimo = 1f;
    public float incrementoVelocidadBorrar = 5f;
    public float velocidadBorrarMaxima = 15f;

    public Transform jugador;
    
    public float posFijaX;
    public float movRango;
    public float velocidad;

    public float intervaloDeAtaq;
    public float radioJugador;
    public float velocidadBorrar;
    public float tiempoBorrar;

    private float movDireccion = 1f;
    private float movCentroY;
    private float attackTimer;
    private enum State {
                entrando,
                buscando, 
                targetVertical, 
                barridoHorizontal, 
                borrando, 
                volviendo}

    private State cntState = State.entrando;

    private HOErasablePlatform plataformaTarget;
    private Vector3 inicioBarrido;
    private Vector3 finalBarrido;
    private int coinsActuales;
    private float timeActual;

    
    /// Inicializa las recompensas del enemigo.
    
    void Awake()
    {
        coinsActuales = coinsBase;
        timeActual = timeBase;
    }

    
    /// Configura las referencias y estado inicial.
    
    void Start()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("HOPlayer");
            if (p != null) jugador = p.transform;
        }

        attackTimer = intervaloDeAtaq;
        movCentroY = transform.position.y;
    }

    
    /// Ejecuta la máquina de estados del borrador.
    
    void Update()
    {
        if (HOScrollingCamera.Instance != null)
        {
            movCentroY += HOScrollingCamera.Instance.CurrentSpeed * Time.deltaTime;
        }

        switch (cntState)
        {
            case State.entrando:
                entering();
                break;
            case State.buscando:
                searchPlayer();
                countdownAttack();
                break;
            case State.targetVertical:
                alingVertical();
                break;
            case State.barridoHorizontal:
                sweepHorizontal();
                break;
            case State.borrando:
                break;
            case State.volviendo:
                returnToSearchPlayer();
                break;
        }
    }

    
    /// Realiza el patrullaje vertical buscando al jugador.
    
    void searchPlayer()
    {
        float minY = movCentroY - movRango;
        float maxY = movCentroY + movRango;

        Vector3 pos = transform.position;
        pos.y += movDireccion * velocidad * Time.deltaTime;

        if (pos.y >= maxY)
        {
            pos.y = maxY;
            movDireccion = -1f;
        }
        else if (pos.y <= minY)
        {
            pos.y = minY;
            movDireccion = 1f;
        }

        transform.position = pos;
    }

    
    /// Reduce el temporizador para el siguiente ataque.
    
    void countdownAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            TryStartAttack();
        }
    }

    
    /// Intenta iniciar un ataque hacia una plataforma cercana al jugador.
    
    void TryStartAttack()
    {
        if (jugador == null) 
        {
            return;
        }

        var nearby = HOPlatformRegistry.getPlatformsNear(jugador.position, radioJugador);
        if (nearby.Count == 0)
        {
            attackTimer = 1f;
            return;
        }

        plataformaTarget = nearby[Random.Range(0, nearby.Count)];
        getPuntosBorrado(plataformaTarget, out inicioBarrido, out finalBarrido);

        cntState = State.targetVertical;
    }

    
    /// Alinea verticalmente al enemigo con la plataforma objetivo.
    
    void alingVertical()
    {
        if (plataformaTarget == null || plataformaTarget.IsErased)
        {
            cntState = State.volviendo;
            return;
        }

        float newY = Mathf.MoveTowards(transform.position.y, inicioBarrido.y, velocidadBorrar * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(transform.position.y - inicioBarrido.y) < 0.05f)
        {
            transform.position = new Vector3(transform.position.x, inicioBarrido.y, transform.position.z);
            cntState = State.barridoHorizontal;

            plataformaTarget.Erase();
        }
    }

    
    /// Realiza un barrido horizontal para borrar la plataforma.
    
    void sweepHorizontal()
    {
        float newX = Mathf.MoveTowards(transform.position.x, finalBarrido.x, velocidadBorrar * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        if (Mathf.Abs(transform.position.x - finalBarrido.x) < 0.05f)
        {
            cntState = State.borrando;
            StartCoroutine(returning());
        }
    }

    
    /// Pausa antes de que el enemigo regrese a su posición.
    
    IEnumerator returning()
    {
        yield return new WaitForSeconds(tiempoBorrar);
        cntState = State.volviendo;
    }

    
    /// Retorna al enemigo a su posición de búsqueda inicial.
    
    void returnToSearchPlayer()
    {
        Vector3 returnTarget = new Vector3(posFijaX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, returnTarget, velocidadBorrar * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - posFijaX) < 0.05f)
        {
            transform.position = new Vector3(posFijaX, transform.position.y, transform.position.z);
            attackTimer = intervaloDeAtaq;
            plataformaTarget = null;
            cntState = State.buscando;
        }
    }

    
    /// Calcula los puntos de inicio y fin del borrado sobre la plataforma.
    
    private void getPuntosBorrado(HOErasablePlatform platform, out Vector3 entryPoint, out Vector3 exitPoint)
    {
        Collider2D col = platform.GetComponent<Collider2D>();
        Bounds bounds = col.bounds;

        float sweepY = bounds.max.y;

        entryPoint = new Vector3(bounds.max.x, sweepY, transform.position.z);

        exitPoint = new Vector3(bounds.min.x - 1f, sweepY, transform.position.z);
    }

    
    /// Ajusta la dificultad del enemigo según el nivel.
    
    public void SetDifficulty(int level)
    {
        velocidad = Mathf.Min(velocidad + level * incrementoVelocidad, velocidadMaxima);

        intervaloDeAtaq = Mathf.Max(intervaloDeAtaq - level * decrementoIntervalo, intervaloMinimo);

        attackTimer = intervaloDeAtaq;

        velocidadBorrar = Mathf.Min(velocidadBorrar + level * incrementoVelocidadBorrar, velocidadBorrarMaxima);
        
        coinsActuales = Mathf.Min(coinsBase + level * incrementoCoins, coinsMaximo);
        timeActual = Mathf.Min(timeBase + level * incrementoTime, timeMaximo);
    }

    
    /// Controla la animación de entrada a la escena.
    
    void entering()
    {
        float newX = Mathf.MoveTowards(transform.position.x, posFijaX, velocidad * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        if (Mathf.Abs(transform.position.x - posFijaX) < 0.05f)
        {
            transform.position = new Vector3(posFijaX, transform.position.y, transform.position.z);
            cntState = State.buscando;
        }
    }

    
    /// Devuelve las monedas que otorga el enemigo al ser derrotado.
    
    public int GetCoinsReward()
    {
        return coinsActuales;
    }

    
    /// Devuelve el tiempo extra que otorga el enemigo.
    
    public float GetTimeReward()
    {
        return timeActual;
    }
}