using UnityEngine;


/// Controla la aparición de enemigos y aumenta su dificultad progresivamente.

public class HOEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnX;
    public float spawnYOffsetFromCamera;

    public float respawnDelay;

    private GameObject currentEnemy;
    private float respawnTimer;
    private bool waitingToRespawn = false;

    private int currentDifficultyLevel;

    
    /// Inicializa la dificultad y genera el primer enemigo.
    
    void Start()
    {
        currentDifficultyLevel = 0;
        SpawnEnemy();
    }

    
    /// Verifica si el enemigo actual fue destruido para iniciar el temporizador de reaparición.
    
    void Update()
    {
        if (currentEnemy == null && !waitingToRespawn)
        {
            waitingToRespawn = true;
            respawnTimer = respawnDelay;
        }

        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                SpawnEnemy();
                waitingToRespawn = false;
            }
        }
    }

    
    /// Instancia un nuevo enemigo y ajusta su dificultad.
    
    void SpawnEnemy()
    {
        Vector3 spawnPos = GetSpawnPosition();
        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        IHOScalableEnemy scalable = currentEnemy.GetComponent<IHOScalableEnemy>();
        
        scalable.SetDifficulty(currentDifficultyLevel);
        
        currentDifficultyLevel++;
    }

    
    /// Calcula la posición de aparición basada en la cámara o posición actual.
    
    Vector3 GetSpawnPosition()
    {
        float spawnY;
        if (HOScrollingCamera.Instance != null)
        {
            spawnY = HOScrollingCamera.Instance.transform.position.y + spawnYOffsetFromCamera;
        }
        else
        {
            spawnY = transform.position.y;
        }
        return new Vector3(spawnX, spawnY, 0f);
    }

    
    /// Reduce la dificultad actual en un porcentaje dado.
    
    public void ReduceDifficulty(float percent)
    {
        percent = Mathf.Clamp01(percent);
        currentDifficultyLevel = Mathf.RoundToInt(currentDifficultyLevel * (1f - percent));
    }

}