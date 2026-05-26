using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LEConveyorManager : MonoBehaviour
{
    // ... Tus variables de control estructurales de la cinta ...
    [SerializeField] private Transform spawnPoint;         
    [SerializeField] private Transform endpointReference;   
    [SerializeField] private float itemSeparation = 1.8f;
    [SerializeField] private Vector2 moveDirection = Vector2.right; 
    [SerializeField] private TextMeshProUGUI timerTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh; 
    [SerializeField] private float gameDurationSeconds = 300f; 
    [SerializeField] private LEAppliance appliancePrefab;
    [SerializeField] private GameObject pausePanel;
    
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
    private readonly Quaternion flatObjectsRotation = Quaternion.Euler(0f, 0f, 90f);

    // NUEVA VARIABLE DE CONTROL
    private bool isPaused = false; 

    // ====================================================================
    // NÚCLEO DEL SISTEMA DE PAUSA (⌐■_幫)
    // ====================================================================
    
    public void PauseGame()
    {
        if (!gameActive || isPaused) return;
        isPaused = true;

        // 1. Congelamos a Gelly de inmediato (animación y lógicas complejas)
        if (gellyController != null) gellyController.PauseCharacter();

        // 2. Buscamos y congelamos todos los electrodomésticos activos de forma directa
        LEAppliance[] activeAppliances = FindObjectsByType<LEAppliance>();
        for (int i = 0; i < activeAppliances.Length; i++)
        {
            activeAppliances[i].SetPauseState(true);
        }
        pausePanel.gameObject.SetActive(true);
        Debug.Log(" Juego pausado correctamente. Flujos congelados.");
    }

    public void ResumeGame()
    {
        if (!gameActive || !isPaused) return;
        isPaused = false;

        // 1. Reanudamos a Gelly restaurando su velocidad exacta
        if (gellyController != null) gellyController.ResumeCharacter();

        // 2. Descongelamos los electrodomésticos activos
        LEAppliance[] activeAppliances = FindObjectsByType<LEAppliance>();
        for (int i = 0; i < activeAppliances.Length; i++)
        {
            activeAppliances[i].SetPauseState(false);
        }

        pausePanel.gameObject.SetActive(false);
        Debug.Log(" Juego reanudado. Flujos reactivados.");
    }

    void Update()
    {
        // Si el juego terminó O está en pausa, el Update no procesa nada. Rendimiento óptimo.
        if (!gameActive || isPaused) return;

        HandleTimer();
        HandleTemporalSpawns();
    }

    // ... El resto de tus métodos (InitializeConveyorGameplay, StartGame, HandleTimer, HandleTemporalSpawns, SpawnNewAppliance, etc.) se quedan exactamente IGUALES ...
    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController; // Asegúrate de arrastrarlo en el Inspector

    public void InitializeConveyorGameplay()
    {
        InitializePool();
        StartGame();
    }

    private void InitializePool()
    {
        for (int i = 0; i < totalSpawnedLimit; i++)
        {
            LEAppliance obj = Instantiate(appliancePrefab, Vector3.zero, flatObjectsRotation, transform);
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null) { rt.localScale = Vector3.one; rt.anchoredPosition = Vector2.zero; }
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

    public float GetRemainingTime()
    {
        return gameTimer;
    }

    public int GetRepairedCount()
    {
        return repairedCount;
    }

    public int GetDiscardedCount()
    {
        return discardedCount;
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
            return;
        }
        SpawnNewAppliance();
        CheckMatchEndCondition();
    }

    private void ReturnToPool(LEAppliance appliance)
    {
        appliance.gameObject.SetActive(false);
        appliance.currentState = LEAppliance.ApplianceState.InPool;
        appliance.transform.rotation = flatObjectsRotation;
        objectPool.Enqueue(appliance);
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < conveyorQueue.Count; i++)
        {
            Vector3 targetPos = endpointReference.position - (Vector3)(moveDirection * (i * itemSeparation));
            conveyorQueue[i].SetTargetPosition(targetPos);
        }
    }

    private void UpdateScoreUI()
    {
        int currentN = totalSpawnedLimit - discardedCount;
        if (scoreTextMesh != null) scoreTextMesh.text = string.Format("{0} / {1}", repairedCount, currentN);
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
            return;
        }
        if (repairedCount >= 1)
        {
            if (scoreTextMesh != null) scoreTextMesh.text = "¡VICTORIA!";
        }
        else
        {
            if (scoreTextMesh != null) scoreTextMesh.text = "¡PERDISTE!";
        }
    }
}
