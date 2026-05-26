using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LEConveyorManager : MonoBehaviour
{
    [Header("Conveyor Configuration")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endpointReference;
    [SerializeField] private float itemSeparation = 1.8f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;

    [Header("UI Score & Timer Elements")]
    [SerializeField] private TextMeshProUGUI timerTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh; // Muestra el (X / N) en tiempo real
    [SerializeField] private float gameDurationSeconds = 300f; // 5 Minutos Max

    [Header("Pool Settings (Strict Limit: 5)")]
    [SerializeField] private LEAppliance appliancePrefab;
    
    private List<LEAppliance> conveyorQueue = new List<LEAppliance>();
    private Queue<LEAppliance> objectPool = new Queue<LEAppliance>();
    
    // Reglas del juego controladas numéricamente
    private int totalSpawnedLimit = 5;
    private int currentSpawnedCount = 0;
    
    private int repairedCount = 0;
    private int discardedCount = 0;

    private float gameTimer;
    private float automaticSpawnTimer;
    private bool gameActive = false;

    private int cachedMinutes = -1;
    private int cachedSeconds = -1;

    // En LEConveyorManager.cs, REEMPLAZA el void Start() por esto:
    public void InitializeConveyorGameplay()
    {
        InitializePool();
        StartGame();
    }

    private void InitializePool()
    {
        // Al ser máximo 5 objetos por partida, el pool es diminuto y ultra-eficiente
        for (int i = 0; i < totalSpawnedLimit; i++)
        {
            LEAppliance obj = Instantiate(appliancePrefab, Vector3.zero, Quaternion.identity, transform);
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
        SpawnNewAppliance(); // Primer electrodoméstico obligatorio
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
        // Candado estricto: Si ya inyectamos los 5 electros de la partida, la cinta no genera más
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
            UpdateQueuePositions(); // Al ser válido, los de atrás avanzan para tapar el bache (o^^)o
        }
    }

    public void RegisterRepair(LEAppliance appliance)
    {
        repairedCount++;
        ReturnToPool(appliance);
        UpdateScoreUI();
        
        // REGLA: Cada que se repare uno con éxito, sale el siguiente inmediatamente
        SpawnNewAppliance();
        CheckMatchEndCondition();
    }

    public void RegisterDiscard(LEAppliance appliance)
    {
        discardedCount++;
        ReturnToPool(appliance);
        UpdateScoreUI();

        // REGLA: Si descarta los 5, pierde en automático de forma fulminante (0/0)
        if (discardedCount >= totalSpawnedLimit)
        {
            gameActive = false;
            Debug.Log("💔 PERDISTE: Descartaste todos los electrodomésticos de la partida.");
            return;
        }

        // Al descartar uno, la cinta debe avanzar metiendo otro si queda en el límite
        SpawnNewAppliance();
        CheckMatchEndCondition();
    }

    private void ReturnToPool(LEAppliance appliance)
    {
        appliance.gameObject.SetActive(false);
        appliance.currentState = LEAppliance.ApplianceState.InPool;
        objectPool.Enqueue(appliance);
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < conveyorQueue.Count; i++)
        {
            Vector3 targetPos = endpointReference.position + (Vector3)(moveDirection * (i * itemSeparation));
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
        // Si procesamos los 5 elementos totales entre reparados y descartados, la partida termina
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
            Debug.Log("💔 ¡TIEMPO AGOTADO! Perdiste.");
            return;
        }

        // Condición de Victoria Dinámica: Requiere por lo menos (1 / N) para ganar
        if (repairedCount >= 1)
        {
            int maxPossibleScore = totalSpawnedLimit; // 5
            int finalScore = repairedCount; 
            Debug.Log($" Ganaste la partida. Puntuación: {finalScore} de {maxPossibleScore} puntos posibles. (o^^)o");
        }
        else
        {
            Debug.Log("💔 Perdiste: No lograste reparar ni un solo electrodoméstico.");
        }
    }
}
