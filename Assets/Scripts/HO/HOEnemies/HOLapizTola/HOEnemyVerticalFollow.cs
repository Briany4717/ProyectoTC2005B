using UnityEngine;

public class HOEnemyVerticalFollow : MonoBehaviour
{
    public Transform player;
    public float posFijaX = -8f;
    // Layermask para detectar plataformas
    public LayerMask platformLayer;
    public float raycastDist = 20f;
    public float velocidad = 10f;
    public float timeEntreSalto = 0.2f;

    private float targetY;
    private float lastRepositionTime;

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