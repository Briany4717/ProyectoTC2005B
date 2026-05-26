using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LEConveyorManager : MonoBehaviour
{
    [Header("Conveyor Configuration")]
    [SerializeField] private Transform spawnPoint;         // Ahora debe estar a la IZQUIERDA
    [SerializeField] private Transform endpointReference;   // Ahora debe estar a la DERECHA
    [SerializeField] private float itemSeparation = 1.8f;
    [Tooltip("Dirección del flujo de la cinta. Ahora fijada de izquierda a derecha.")]
    [SerializeField] private Vector2 moveDirection = Vector2.right; // <--- CAMBIO: Flujo hacia la derecha

    [Header("UI Score & Timer Elements")]
    [SerializeField] private TextMeshProUGUI timerTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh; 
    [SerializeField] private float gameDurationSeconds = 300f; 

    [Header("Pool Settings (Strict Limit: 5)")]
    [SerializeField] private LEAppliance appliancePrefab;
    
    private List<LEAppliance> conveyorQueue = new List<LEAppliance>();
    private Queue<LEAppliance> objectPool = new Queue<LEAppliance>();
    
    private int totalSpawnedLimit = 5;
    private int currentSpawnedCount = 0;
    private int repairedCount = 0;
    private int discardedCount = 0;

    private float gameTimer;
    private float automaticSpawnTimer;
    private bool gameActive = false;

    private int cachedMinutes = -1;
    private int cachedSeconds = -1;

    // EL TRUCO DE ROTACIÓN (⌐■_■): Los objetos nacen acostados en Z = 90
    private readonly Quaternion flatObjectsRotation = Quaternion.Euler(0f, 0f, 90f);

    public void InitializeConveyorGameplay()
    {
        InitializePool();
        StartGame();
    }

    private void InitializePool()
    {
        for (int i = 0; i < totalSpawnedLimit; i++)
        {
            // ¡AJUSTE CRUCIAL!: Inyectamos la rotación Z = 90 en el frame de nacimiento
            LEAppliance obj = Instantiate(appliancePrefab, Vector3.zero, flatObjectsRotation, transform);
            
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one; 
                rt.anchoredPosition = Vector2.zero;
            }

            obj.gameObject.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public void StartGame()
    {
        gameTimer = gameDurationSeconds;
        automaticSpawnTimer = 0f;
        repairedCount = 0;
        discardedCount = 0;
        currentSpawnedCount = 0;
        gameActive = true;

        UpdateScoreUI();
        SpawnNewAppliance(); 
    }

    void Update()
    {
        if (!gameActive) return;

        HandleTimer();
        HandleTemporalSpawns();
    }

    private void HandleTimer()
    {
        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0f)
        {
            gameTimer = 0f;
            gameActive = false;
            EvaluateMatchResult(timeOut: true);
            return;
        }

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);

        if (minutes != cachedMinutes || seconds != cachedSeconds)
        {
            cachedMinutes = minutes;
            cachedSeconds = seconds;
            if (timerTextMesh != null) timerTextMesh.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void HandleTemporalSpawns()
    {
        automaticSpawnTimer += Time.deltaTime;
        if (automaticSpawnTimer >= 30f)
        {
            automaticSpawnTimer = 0f;
            SpawnNewAppliance();
        }
    }

    public void SpawnNewAppliance()
    {
        if (currentSpawnedCount >= totalSpawnedLimit) return;

        if (objectPool.Count > 0)
        {
            LEAppliance appliance = objectPool.Dequeue();
            appliance.gameObject.SetActive(true);
            appliance.SetupInConveyor(spawnPoint.position);
            
            conveyorQueue.Add(appliance);
            currentSpawnedCount++;
            
            UpdateQueuePositions();
        }
    }

    public void RemoveFromConveyor(LEAppliance appliance)
    {
        if (conveyorQueue.Contains(appliance))
        {
            conveyorQueue.Remove(appliance);
            UpdateQueuePositions(); 
        }
    }

    public void RegisterRepair(LEAppliance appliance)
    {
        repairedCount++;
        ReturnToPool(appliance);
        UpdateScoreUI();
        
        SpawnNewAppliance();
        CheckMatchEndCondition();
    }

    public void RegisterDiscard(LEAppliance appliance)
    {
        discardedCount++;
        ReturnToPool(appliance);
        UpdateScoreUI();

        if (discardedCount >= totalSpawnedLimit)
        {
            gameActive = false;
            if (scoreTextMesh != null) scoreTextMesh.text = "¡PERDISTE!";
            Debug.Log("PERDISTE: Descartaste todos los electrodomésticos de la partida.");
            return;
        }

        SpawnNewAppliance();
        CheckMatchEndCondition();
    }

    private void ReturnToPool(LEAppliance appliance)
    {
        appliance.gameObject.SetActive(false);
        appliance.currentState = LEAppliance.ApplianceState.InPool;
        
        // Al regresar al pool, nos aseguramos de restaurar su rotación Z = 90 original de fábrica
        appliance.transform.rotation = flatObjectsRotation;
        
        objectPool.Enqueue(appliance);
    }

    private void UpdateQueuePositions()
    {
        // ====================================================================
        // MATEMÁTICA DE RETRO-ACUMULACIÓN (Izquierda a Derecha)
        // Como el flujo va a la derecha, los objetos que vienen atrás deben
        // apilarse restando (-) el vector de dirección para hacer fila hacia la izquierda.
        // ====================================================================
        for (int i = 0; i < conveyorQueue.Count; i++)
        {
            Vector3 targetPos = endpointReference.position - (Vector3)(moveDirection * (i * itemSeparation));
            conveyorQueue[i].SetTargetPosition(targetPos);
        }
    }

    private void UpdateScoreUI()
    {
        int currentN = totalSpawnedLimit - discardedCount;
        if (scoreTextMesh != null)
        {
            scoreTextMesh.text = string.Format("{0} / {1}", repairedCount, currentN);
        }
    }

    private void CheckMatchEndCondition()
    {
        if (repairedCount + discardedCount >= totalSpawnedLimit)
        {
            gameActive = false;
            EvaluateMatchResult(timeOut: false);
        }
    }

    private void EvaluateMatchResult(bool timeOut)
    {
        if (timeOut)
        {
            if (scoreTextMesh != null) scoreTextMesh.text = "¡TIEMPO AGOTADO!";
            Debug.Log("¡TIEMPO AGOTADO!");
            return;
        }

        if (repairedCount >= 1)
        {
            int maxPossibleScore = totalSpawnedLimit; 
            int finalScore = repairedCount; 
            if (scoreTextMesh != null) scoreTextMesh.text = "¡VICTORIA!";
            Debug.Log($" Ganaste la partida. Puntuación: {finalScore} de {maxPossibleScore} puntos posibles. (o^^)o");
        }
        else
        {
            if (scoreTextMesh != null) scoreTextMesh.text = "¡PERDISTE!";
            Debug.Log("Perdiste: No lograste reparar ni un solo electrodoméstico.");
        }
    }
}
