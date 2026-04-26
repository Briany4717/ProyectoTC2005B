using System.Collections;
using UnityEngine;

public class HOEraserEnemy : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Patrulla vertical")]
    [Tooltip("Posición X fija mientras patrulla (lado derecho)")]
    public float patrolX = 8f;
    [Tooltip("Y mínima de la patrulla")]
    public float patrolMinY = -3f;
    [Tooltip("Y máxima de la patrulla")]
    public float patrolMaxY = 5f;
    [Tooltip("Velocidad de patrulla vertical")]
    public float patrolSpeed = 2f;

    [Header("Ataque")]
    [Tooltip("Cada cuánto intenta borrar una plataforma")]
    public float attackInterval = 6f;
    [Tooltip("Radio alrededor del jugador para elegir plataforma")]
    public float searchRadius = 6f;
    [Tooltip("Velocidad cuando se desplaza a borrar")]
    public float attackMoveSpeed = 8f;
    [Tooltip("Tiempo que permanece sobre la plataforma borrándola")]
    public float eraseHoldTime = 0.5f;

    private float patrolDirection = 1f;
    private float attackTimer;
    private enum State { Patrolling, MovingToTarget, Erasing, Returning }
    private State currentState = State.Patrolling;

    private HOErasablePlatform targetPlatform;
    private Vector3 patrolReturnPosition;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("HOPlayer");
            if (p != null) player = p.transform;
        }

        attackTimer = attackInterval;
        transform.position = new Vector3(patrolX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                CountdownAttack();
                break;
            case State.MovingToTarget:
                MoveToTarget();
                break;
            case State.Erasing:
                // El borrado lo maneja la corutina
                break;
            case State.Returning:
                ReturnToPatrol();
                break;
        }
    }

    void Patrol()
    {
        Vector3 pos = transform.position;
        pos.y += patrolDirection * patrolSpeed * Time.deltaTime;

        if (pos.y >= patrolMaxY)
        {
            pos.y = patrolMaxY;
            patrolDirection = -1f;
        }
        else if (pos.y <= patrolMinY)
        {
            pos.y = patrolMinY;
            patrolDirection = 1f;
        }

        transform.position = pos;
    }

    void CountdownAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            TryStartAttack();
        }
    }

    void TryStartAttack()
    {
        if (player == null) return;

        var nearby = HOPlatformRegistry.GetPlatformsNearPlayer(player.position, searchRadius);
        if (nearby.Count == 0)
        {
            // No hay plataformas válidas, espera un poco antes de reintentar
            attackTimer = 1f;
            return;
        }

        targetPlatform = nearby[Random.Range(0, nearby.Count)];
        patrolReturnPosition = transform.position;
        currentState = State.MovingToTarget;
    }

    void MoveToTarget()
    {
        if (targetPlatform == null || targetPlatform.IsErased)
        {
            currentState = State.Returning;
            return;
        }

        Vector3 target = targetPlatform.transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            attackMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentState = State.Erasing;
            StartCoroutine(EraseAndReturn());
        }
    }

    IEnumerator EraseAndReturn()
    {
        targetPlatform.Erase();
        yield return new WaitForSeconds(eraseHoldTime);
        currentState = State.Returning;
    }

    void ReturnToPatrol()
    {
        // Vuelve al carril vertical de patrulla
        Vector3 returnTarget = new Vector3(patrolX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(
            transform.position,
            returnTarget,
            attackMoveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - patrolX) < 0.05f)
        {
            transform.position = new Vector3(patrolX, transform.position.y, transform.position.z);
            attackTimer = attackInterval;
            targetPlatform = null;
            currentState = State.Patrolling;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza el carril de patrulla
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(patrolX, patrolMinY, 0), new Vector3(patrolX, patrolMaxY, 0));

        // Visualiza el radio de búsqueda alrededor del jugador
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(player.position, searchRadius);
        }
    }
}
