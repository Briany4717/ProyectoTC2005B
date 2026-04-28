using System.Collections;
using UnityEngine;

public class HOEraserEnemy : MonoBehaviour
{
    public Transform jugador;
    // Parametros del borrador patrullando
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
    private enum State {buscando, 
                targetVertical, 
                barridoHorizontal, 
                borrando, 
                volviendo}

    // inicialmente va a estar buscando
    private State cntState = State.buscando;

    private HOErasablePlatform plataformaTarget;
    private Vector3 inicioBarrido;
    private Vector3 finalBarrido;

    void Start()
    {
        attackTimer = intervaloDeAtaq;
        movCentroY = transform.position.y;
        transform.position = new Vector3(posFijaX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (HOScrollingCamera.Instance != null)
        {
            movCentroY += HOScrollingCamera.Instance.CurrentSpeed * Time.deltaTime;
        }

        switch (cntState)
        {
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

    void countdownAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            TryStartAttack();
        }
    }

    void TryStartAttack()
    {
        // por si acaso el jugador se nos va de rango
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

        // ataca  a una plataforma random de las que están cerca del jugador
        plataformaTarget = nearby[Random.Range(0, nearby.Count)];
        // calcula de donde a donde va a barrer la plataforma
        getPuntosBorrado(plataformaTarget, out inicioBarrido, out finalBarrido);

        cntState = State.targetVertical;
    }

    void alingVertical()
    {
        // por si ya no hay plataforma
        if (plataformaTarget == null || plataformaTarget.IsErased)
        {
            cntState = State.volviendo;
            return;
        }

        // se ajusta en y
        float newY = Mathf.MoveTowards(transform.position.y, inicioBarrido.y, velocidadBorrar * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // ahora confirmamos si ya está en posición
        if (Mathf.Abs(transform.position.y - inicioBarrido.y) < 0.05f)
        {
            // lo ajustamos exactamente por si acaso
            transform.position = new Vector3(transform.position.x, inicioBarrido.y, transform.position.z);
            cntState = State.barridoHorizontal;

            plataformaTarget.Erase();
        }
    }

    void sweepHorizontal()
    {
        float newX = Mathf.MoveTowards(transform.position.x, finalBarrido.x, velocidadBorrar * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        // checa si ya llegó a la orilla izquierda
        if (Mathf.Abs(transform.position.x - finalBarrido.x) < 0.05f)
        {
            cntState = State.borrando;
            StartCoroutine(returning());
        }
    }

    IEnumerator returning()
    {
        yield return new WaitForSeconds(tiempoBorrar);
        cntState = State.volviendo;
    }

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

    
    private void getPuntosBorrado(HOErasablePlatform platform, out Vector3 entryPoint, out Vector3 exitPoint)
    {
        Collider2D col = platform.GetComponent<Collider2D>();
        Bounds bounds = col.bounds;

        float sweepY = bounds.max.y;

        entryPoint = new Vector3(bounds.max.x, sweepY, transform.position.z);

        exitPoint = new Vector3(bounds.min.x - 1f, sweepY, transform.position.z);
    }
}
