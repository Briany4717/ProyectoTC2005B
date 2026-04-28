using UnityEngine;

public class HOPlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;

    private float yInicial = 5;
    private float posFijaX = 0f;
    private float espacioEntrePlat = 1.5f;
    private float spawnAhead = 1.5f;
    private float lastSpawnedY;

    void Start()
    {
        lastSpawnedY = yInicial;
    }

    void Update()
    {
        float spawnLimit = HOScrollingCamera.Instance.TopEdgeY + spawnAhead;
        while (lastSpawnedY < spawnLimit)
        {
            spawnNextPlatform();
        }
    }

    void spawnNextPlatform()
    {
        float nextY = lastSpawnedY + espacioEntrePlat;
        spawnAt(nextY);
    }


    void spawnAt(float y)
    {
        Vector3 spawnPos = new Vector3(posFijaX, y, 0f);
        Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        lastSpawnedY = y;
    }

}