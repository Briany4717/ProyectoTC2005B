using UnityEngine;

public class HOEnemyVerticalFollow : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del jugador al que debe seguir")]
    public Transform player;

    [Header("Posición Horizontal Fija")]
    [Tooltip("Posición X fija del enemigo. Configura aquí si está a la izquierda o derecha.")]
    public float fixedXPosition = -8f;

    [Header("Detección de Plataforma")]
    [Tooltip("Layer de las plataformas para detectar dónde está parado el jugador")]
    public LayerMask platformLayer;
    [Tooltip("Distancia máxima del raycast hacia abajo desde el jugador")]
    public float raycastDistance = 20f;
    [Tooltip("Offset vertical sobre la plataforma (para que el enemigo no quede enterrado)")]
    public float verticalOffset = 0f;

    [Header("Movimiento")]
    [Tooltip("Si es true, el enemigo se teletransporta. Si es false, se mueve suavemente.")]
    public bool instantSnap = false;
    [Tooltip("Velocidad de movimiento si no es instantáneo")]
    public float moveSpeed = 10f;
    [Tooltip("Tiempo mínimo entre cambios de plataforma para evitar saltos nerviosos")]
    public float repositionCooldown = 0.2f;

    private float targetY;
    private float lastRepositionTime;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("HOPlayer");
            if (p != null) player = p.transform;
        }

        targetY = transform.position.y;
    }

    void Update()
    {
        if (player == null) return;

        // Solo recalcula si pasó el cooldown (evita parpadeos al saltar)
        if (Time.time - lastRepositionTime >= repositionCooldown)
        {
            UpdateTargetPlatform();
            lastRepositionTime = Time.time;
        }

        MoveToTarget();
    }

    void UpdateTargetPlatform()
    {
        // Raycast desde el jugador hacia abajo para encontrar la plataforma
        RaycastHit2D hit = Physics2D.Raycast(
            player.position,
            Vector2.down,
            raycastDistance,
            platformLayer
        );

        if (hit.collider != null)
        {
            // Posiciona al enemigo sobre la plataforma del jugador
            targetY = hit.point.y + verticalOffset;
        }
        else
        {
            // Si el jugador está en el aire (saltando), mantén la última posición válida
            // o sigue su Y directamente como fallback
            targetY = player.position.y;
        }
    }

    void MoveToTarget()
    {
        Vector3 currentPos = transform.position;
        float newY;

        if (instantSnap)
        {
            newY = targetY;
        }
        else
        {
            newY = Mathf.MoveTowards(currentPos.y, targetY, moveSpeed * Time.deltaTime);
        }

        transform.position = new Vector3(fixedXPosition, newY, currentPos.z);
    }

    // Visualización en el editor para debug
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.position, player.position + Vector3.down * raycastDistance);
        }
    }
}