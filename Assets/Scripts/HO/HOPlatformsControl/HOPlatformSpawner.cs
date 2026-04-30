using UnityEngine;

public class HOPlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;

    private float yInicial = 0.5f;
    private float posFijaX = 0f;
    private float espacioEntrePlat = 1.5f;
    private float spawnMargin = 3f;
    private float lastSpawnedY;

    void Start()
    {
        lastSpawnedY = yInicial;
    }

    void Update()
    {
        float spawnLimit = HOScrollingCamera.Instance.topEdge - spawnMargin;
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