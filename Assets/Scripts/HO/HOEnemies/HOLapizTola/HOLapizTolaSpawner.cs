using UnityEngine;

/// <summary>
/// Controla la aparición y reaparición del enemigo lápiz en la escena.
/// </summary>
public class HOLapizTolaSpawner : MonoBehaviour
{
    public GameObject lapizPrefab;
    
    float spawnX = -7f;
    float respawnDelay = 3f;

    private GameObject cntEnemy;
    private float respawnTimer;
    private bool waitingToRespawn = false;

    /// <summary>
    /// Verifica la asignación del prefab e inicia el ciclo de aparición.
    /// </summary>
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

    /// <summary>
    /// Maneja el temporizador para la reaparición del lápiz tras ser derrotado.
    /// </summary>
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

    /// <summary>
    /// Instancia un nuevo lápiz en la posición definida.
    /// </summary>
    void spawnLapiz()
    {
        Vector3 spawnPos = getSpawnPos();
        cntEnemy = Instantiate(lapizPrefab, spawnPos, Quaternion.identity);
    }

    /// <summary>
    /// Calcula la posición de aparición del lápiz.
    /// </summary>
    Vector3 getSpawnPos()
    {
        float spawnY = transform.position.y;
        return new Vector3(spawnX, spawnY, 0f);
    }
}