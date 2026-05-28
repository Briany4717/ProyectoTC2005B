using UnityEngine;


/// Controla la generación infinita de plataformas a medida que la cámara avanza.

public class HOPlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;

    private float yInicial = 0.5f;
    private float posFijaX = 0f;
    private float espacioEntrePlat = 1.5f;
    private float spawnMargin = 3f;
    private float lastSpawnedY;

    
    /// Inicializa la posición de la primera plataforma.
    
    void Start()
    {
        lastSpawnedY = yInicial;
    }

    
    /// Genera nuevas plataformas si el límite superior lo permite.
    
    void Update()
    {
        float spawnLimit = HOScrollingCamera.Instance.topEdge - spawnMargin;
        while (lastSpawnedY < spawnLimit)
        {
            spawnNextPlatform();
        }
    }

    
    /// Calcula la posición para la siguiente plataforma y la instancia.
    
    void spawnNextPlatform()
    {
        float nextY = lastSpawnedY + espacioEntrePlat;
        spawnAt(nextY);
    }

    
    /// Instancia una plataforma en la altura especificada.
    
    void spawnAt(float y)
    {
        Vector3 spawnPos = new Vector3(posFijaX, y, 0f);
        Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        lastSpawnedY = y;
    }
}