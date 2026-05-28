using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LEConveyorManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;         
    [SerializeField] private Transform endpointReference;   
    [SerializeField] private float itemSeparation = 1.8f;
    [SerializeField] private Vector2 moveDirection = Vector2.right; 
    [SerializeField] private TextMeshProUGUI timerTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh; 
    [SerializeField] private float gameDurationSeconds; 
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
    private bool isPaused = false; 

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController; 

    public void InitializeConveyorGameplay()
    {
        InitializePool();
        
        if (LEGameSessionData.Instance.isGameInProgress)
        {
            // 1. Restauramos telemetría básica
            gameTimer = LEGameSessionData.Instance.remainingTime;
            repairedCount = LEGameSessionData.Instance.repairedCount;
            discardedCount = LEGameSessionData.Instance.discardedCount;
            
            // 2. Recuperamos el conteo histórico total de spawns exacto de la sesión
            currentSpawnedCount = LEGameSessionData.Instance.totalSpawnedCount;
            
            gameActive = true;
            UpdateScoreUI();
            
            // ====================================================================
            // 🛠️ ALGORITMO DE RECONSTRUCCIÓN DE COLA DE ALTO RENDIMIENTO  
            // Calculamos cuántos aparatos se quedaron esperando fila en la cinta 
            // antes de irnos a la otra escena. El cálculo matemático exacto es:
            // ConteoHistórico - YaReparados - YaDescartados
            // ====================================================================
            int appliancesLeftWaiting = currentSpawnedCount - repairedCount - discardedCount;
            
            for (int i = 0; i < appliancesLeftWaiting; i++)
            {
                ReconstructApplianceOnConveyor();
            }

            // 3. REGLA DE ORO: Como acabamos de completar una reparación con éxito,
            // la cinta transportadora debe escupir el siguiente objeto de reemplazo de inmediato
            SpawnNewAppliance();
            
            Debug.Log("🔌 [Session Restored] Cola reconstruida y nuevo spawn inyectado.");
        }
        else
        {
            LEGameSessionData.Instance.isGameInProgress = true;
            StartGame();
        }
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
        gameDurationSeconds = PlayerPrefs.GetFloat("LE_Minigame_Duration", 330f);
        gameTimer = gameDurationSeconds;
        LEGameSessionData.Instance.totalMatchDuration = gameDurationSeconds;
        automaticSpawnTimer = 0f;
        repairedCount = 0;
        discardedCount = 0;
        currentSpawnedCount = 0;
        gameActive = true;
        
        UpdateScoreUI();
        SpawnNewAppliance(); 

        // Sincronizamos el conteo inicial en el contenedor estático
        LEGameSessionData.Instance.totalSpawnedCount = currentSpawnedCount;
    }

    /// <summary>
    /// Reconstruye un aparato de la fila vieja sin alterar ni duplicar el conteo histórico general.
    /// </summary>
    private void ReconstructApplianceOnConveyor()
    {
        if (objectPool.Count > 0)
        {
            LEAppliance appliance = objectPool.Dequeue();
            appliance.gameObject.SetActive(true);
            appliance.SetupInConveyor(spawnPoint.position);
            conveyorQueue.Add(appliance);
            
            UpdateQueuePositions();
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
            
            // ¡VITAL!: Guardamos el nuevo conteo en el puente estático para el siguiente viaje  
            LEGameSessionData.Instance.totalSpawnedCount = currentSpawnedCount;
            
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

    public float GetRemainingTime() => gameTimer;
    public int GetRepairedCount() => repairedCount;
    public int GetDiscardedCount() => discardedCount;
    public int GetTotalSpawnedCount() => currentSpawnedCount; // Getter expuesto

    void Update()
    {
        if (!gameActive || isPaused) return;
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
            LEGameSessionData.Instance.isVictory = false;
            SceneManager.LoadScene("LEFinalScene");
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
        gameActive = false;
        var session = LEGameSessionData.Instance;

        if (timeOut)
        {
            session.isVictory = false;
        }
        else
        {
            int totalN = totalSpawnedLimit - discardedCount;
            session.isVictory = (repairedCount >= totalN);
        }

        SceneManager.LoadScene("LEFinalScene");
    }

    public void PauseGame()
    {
        if (!gameActive || isPaused) return;
        isPaused = true;
        if (gellyController != null) gellyController.PauseCharacter();
        LEAppliance[] activeAppliances = FindObjectsByType<LEAppliance>();
        for (int i = 0; i < activeAppliances.Length; i++) activeAppliances[i].SetPauseState(true);
        pausePanel.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!gameActive || !isPaused) return;
        isPaused = false;
        if (gellyController != null) gellyController.ResumeCharacter();
        LEAppliance[] activeAppliances = FindObjectsByType<LEAppliance>();
        for (int i = 0; i < activeAppliances.Length; i++) activeAppliances[i].SetPauseState(false);
        pausePanel.gameObject.SetActive(false);
    }
}
