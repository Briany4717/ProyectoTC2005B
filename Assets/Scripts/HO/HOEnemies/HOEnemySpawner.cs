using UnityEngine;

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

    void Start()
    {
        currentDifficultyLevel = 0;
        SpawnEnemy();
    }

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

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetSpawnPosition();
        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // usé la interfaz para, independientemente del enemigo, subir su dificultad por respawn
        IHOScalableEnemy scalable = currentEnemy.GetComponent<IHOScalableEnemy>();
        
        scalable.SetDifficulty(currentDifficultyLevel);
        
        currentDifficultyLevel++;
    }

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

    public void ReduceDifficulty(float percent)
    {
        percent = Mathf.Clamp01(percent);
        currentDifficultyLevel = Mathf.RoundToInt(currentDifficultyLevel * (1f - percent));
    }

}