using UnityEngine;

public class HOLapizTolaSpawner : MonoBehaviour
{
    public GameObject lapizPrefab;
    
    float spawnX = -7f;
    float respawnDelay = 3f;

    private GameObject cntEnemy;
    private float respawnTimer;
    private bool waitingToRespawn = false;

    void Start()
    {
        if (lapizPrefab == null)
        {
            Debug.LogError("falta el prefab del lapiz");
            enabled = false;
            return;
        }

        spawnLapiz();
    }

    void Update()
    {
        if (cntEnemy == null && !waitingToRespawn)
        {
            waitingToRespawn = true;
            respawnTimer = respawnDelay;
        }

        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                spawnLapiz();
                waitingToRespawn = false;
            }
        }
    }

    void spawnLapiz()
    {
        Vector3 spawnPos = getSpawnPos();
        cntEnemy = Instantiate(lapizPrefab, spawnPos, Quaternion.identity);
    }

    Vector3 getSpawnPos()
    {
        float spawnY = transform.position.y;
        return new Vector3(spawnX, spawnY, 0f);
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza la posición de spawn
        Gizmos.color = Color.red;
        Vector3 pos = getSpawnPos();
        Gizmos.DrawWireSphere(pos, 0.5f);
    }
}