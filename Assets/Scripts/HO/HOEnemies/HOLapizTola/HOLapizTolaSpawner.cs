using UnityEngine;


/// Controla la aparición y reaparición del enemigo lápiz en la escena.

public class HOLapizTolaSpawner : MonoBehaviour
{
    public GameObject lapizPrefab;
    
    float spawnX = -7f;
    float respawnDelay = 3f;

    private GameObject cntEnemy;
    private float respawnTimer;
    private bool waitingToRespawn = false;

    
    /// Verifica la asignación del prefab e inicia el ciclo de aparición.
    
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

    
    /// Maneja el temporizador para la reaparición del lápiz tras ser derrotado.
    
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

    
    /// Instancia un nuevo lápiz en la posición definida.
    
    void spawnLapiz()
    {
        Vector3 spawnPos = getSpawnPos();
        cntEnemy = Instantiate(lapizPrefab, spawnPos, Quaternion.identity);
    }

    
    /// Calcula la posición de aparición del lápiz.
    
    Vector3 getSpawnPos()
    {
        float spawnY = transform.position.y;
        return new Vector3(spawnX, spawnY, 0f);
    }
}