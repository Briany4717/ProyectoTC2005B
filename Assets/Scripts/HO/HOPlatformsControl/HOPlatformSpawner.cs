using UnityEngine;

/// <summary>
/// Controla la generación infinita de plataformas a medida que la cámara avanza.
/// </summary>
public class HOPlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;

    private float yInicial = 0.5f;
    private float posFijaX = 0f;
    private float espacioEntrePlat = 1.5f;
    private float spawnMargin = 3f;
    private float lastSpawnedY;

    /// <summary>
    /// Inicializa la posición de la primera plataforma.
    /// </summary>
    void Start()
    {
        lastSpawnedY = yInicial;
    }

    /// <summary>
    /// Genera nuevas plataformas si el límite superior lo permite.
    /// </summary>
    void Update()
    {
        float spawnLimit = HOScrollingCamera.Instance.topEdge - spawnMargin;
        while (lastSpawnedY < spawnLimit)
        {
            spawnNextPlatform();
        }
    }

    /// <summary>
    /// Calcula la posición para la siguiente plataforma y la instancia.
    /// </summary>
    void spawnNextPlatform()
    {
        float nextY = lastSpawnedY + espacioEntrePlat;
        spawnAt(nextY);
    }

    /// <summary>
    /// Instancia una plataforma en la altura especificada.
    /// </summary>
    void spawnAt(float y)
    {
        Vector3 spawnPos = new Vector3(posFijaX, y, 0f);
        Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        lastSpawnedY = y;
    }
}