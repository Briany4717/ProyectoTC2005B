using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LERepairManager : MonoBehaviour
{
    public enum RepairState { IntroDialogue, RepairingAnimation, GameplayActive, ProblemDialogue, ExecutingMinigame, Paused }

    [Header("Current State")]
    public RepairState currentState = RepairState.IntroDialogue;

    [Header("UI Main Connections")]
    [SerializeField] private Image applianceMainImage;
    [SerializeField] private TextMeshProUGUI globalTimerTextMesh;
    [SerializeField] private GameObject pausePanel;

    [Header("Gelly Intro Chat Bubble")]
    [SerializeField] private GameObject gellyIntroChatBubble;
    [SerializeField] private TextMeshProUGUI gellyIntroTextMesh;

    [Header("Gelly Floating Problem Bubble (?)")]
    [SerializeField] private GameObject gellyProblemBubbleContainer;
    [SerializeField] private RectTransform problemBubbleRect;
    [SerializeField] private float floatSpeed = 4f;
    [SerializeField] private float floatIntensity = 15f;

    [Header("Independent Problem Panel")]
    [SerializeField] private GameObject problemDialogueOverlayPanel;
    [SerializeField] private TextMeshProUGUI overlayTitleText;
    [SerializeField] private TextMeshProUGUI overlayBodyText;

    [Header("Instruction Sheet")]
    [SerializeField] private TextMeshProUGUI[] taskTextMeshes; // Estrictamente 3

    [Header("Strikes UI")]
    [SerializeField] private TextMeshProUGUI strikesTextMesh;

    [Header("Procedural Debris Settings (⌐■_■)")]
    [SerializeField] private float repairAnimDuration = 2.5f;
    [SerializeField] private float shakeSpeed = 80f;
    [SerializeField] private float shakeIntensity = 7.0f;
    [SerializeField] private GameObject debrisUIPrefab; // Prefab con el script LEDebrisPhysics
    [SerializeField] private Transform debrisSpawnPoint;
    [SerializeField] private Sprite[] debrisSpritesPool; // Lista de tornillos/tuercas en el inspector
    [SerializeField] private float timeBetweenDebrisSpawns = 0.08f;

    [Header("Audio (Rejection / Success)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip rejectionSFX;

    private LEApplianceRepairData currentData;
    private Vector3 applianceOriginalPosition;
    private Vector3 problemBubbleOriginalAnchoredPos;
    private int currentTaskIndex = 0;
    private bool isMatchActive = false;
    private int cachedMinutes = -1;
    private int cachedSeconds = -1;
    private RepairState stateBeforePause;
    
    private Coroutine wrongToolFeedbackCoroutine;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (problemDialogueOverlayPanel != null) problemDialogueOverlayPanel.SetActive(false);
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(false);
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);

        applianceOriginalPosition = applianceMainImage.transform.position;
        if (problemBubbleRect != null) problemBubbleOriginalAnchoredPos = problemBubbleRect.anchoredPosition;

        LoadPersistedSessionData();
        InitializeRepairFlow();
    }

    private void LoadPersistedSessionData()
    {
        if (applianceMainImage != null && LEGameSessionData.Instance.currentApplianceSprite != null)
        {
            applianceMainImage.sprite = LEGameSessionData.Instance.currentApplianceSprite;
        }

        int currentDataIndex = LEGameSessionData.Instance.currentMatchDataIndex;
        currentData = LEGameSessionData.Instance.currentMatchData[currentDataIndex];

        for (int i = 0; i < taskTextMeshes.Length; i++)
        {
            if (i < currentData.tasks.Length) taskTextMeshes[i].text = currentData.tasks[i];
        }

        UpdateStrikesUI();
        isMatchActive = true;
    }

    private void InitializeRepairFlow()
    {
        currentState = RepairState.IntroDialogue;
        if (gellyIntroChatBubble != null)
        {
            gellyIntroChatBubble.SetActive(true);
            gellyIntroTextMesh.text = Random.Range(0, 2) == 0 ? "¡Manos a la obra con esto!" : "¡Comencemos la reparación ya!";
        }
    }

    public void AdvanceFromIntro()
    {
        if (currentState != RepairState.IntroDialogue) return;
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);

        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    private IEnumerator ExecuteRepairAnimationRoutine()
    {
        currentState = RepairState.RepairingAnimation;
        
        // Escondemos las burbujas durante la animación de reparación por diseño
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(false);
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);

        float timer = 0f;
        float debrisTimer = 0f;

        while (timer < repairAnimDuration)
        {
            if (currentState != RepairState.Paused)
            {
                timer += Time.deltaTime;
                debrisTimer += Time.deltaTime;

                // 1. Sacudido procedimental puro
                float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
                applianceMainImage.transform.position = applianceOriginalPosition + new Vector3(shakeX, 0f, 0f);

                // 2. Lanzador de Chatarra a trayectorias aleatorias por código (0 Allocations de partículas)
                if (debrisTimer >= timeBetweenDebrisSpawns && debrisSpritesPool.Length > 0 && debrisUIPrefab != null)
                {
                    debrisTimer = 0f;
                    GameObject debrisObj = Instantiate(debrisUIPrefab, transform); // Se instancia bajo el canvas
                    LEDebrisPhysics debrisScript = debrisObj.GetComponent<LEDebrisPhysics>();
                    
                    Sprite randomDebrisSprite = debrisSpritesPool[Random.Range(0, debrisSpritesPool.Length)];
                    debrisScript.InitializeDebris(randomDebrisSprite, debrisSpawnPoint.position);
                }
            }
            yield return null;
        }

        applianceMainImage.transform.position = applianceOriginalPosition;

        if (currentTaskIndex >= 3)
        {
            EvaluateApplianceFixConclusion();
            yield break;
        }

        // Habilitamos el Gameplay y despertamos la burbuja del "?"
        currentState = RepairState.GameplayActive;
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(true);
    }

    void Update()
    {
        if (!isMatchActive || currentState == RepairState.Paused) return;

        HandleRemainingGlobalTimer();
        HandleProblemBubbleFloatingEffect();
    }

    private void HandleProblemBubbleFloatingEffect()
    {
        // Efecto elástico y armónico de globo flotando de arriba a abajo de forma perpetua (o^^)o
        if (currentState == RepairState.GameplayActive && problemBubbleRect != null && gellyProblemBubbleContainer.activeSelf)
        {
            float newY = problemBubbleOriginalAnchoredPos.y + (Mathf.Sin(Time.time * floatSpeed) * floatIntensity);
            problemBubbleRect.anchoredPosition = new Vector2(problemBubbleOriginalAnchoredPos.x, newY);
        }
    }

    public void OnClickProblemBubble()
    {
        if (currentState != RepairState.GameplayActive) return;
        
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(false);
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);

        currentState = RepairState.ProblemDialogue;
        if (problemDialogueOverlayPanel != null) problemDialogueOverlayPanel.SetActive(true);

        if (overlayTitleText != null) overlayTitleText.text = currentData.gellyDialogue;
        if (overlayBodyText != null) overlayBodyText.text = currentData.problemText;
    }

    public void CloseProblemDialogueWindow()
    {
        if (currentState != RepairState.ProblemDialogue) return;
        if (problemDialogueOverlayPanel != null) problemDialogueOverlayPanel.SetActive(false);

        currentState = RepairState.GameplayActive;
        
        // SE ACLARA: Al cerrar el panel, el globo del "?" vuelve a emerger para consultas futuras
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(true);
    }

    public void ProcessToolDropped(int toolId)
    {
        // Las herramientas siempre pueden arrastrarse en Gameplay Activo
        if (currentState != RepairState.GameplayActive) return;

        if (toolId == currentData.correctToolId)
        {
            if (wrongToolFeedbackCoroutine != null) StopCoroutine(wrongToolFeedbackCoroutine);
            applianceMainImage.color = Color.white;

            currentState = RepairState.ExecutingMinigame;
            
            // =========================================================
            // PASO PREPARADO: Aquí se conectará tu minijuego interactivo.
            // Simulación temporal de victoria automática para testeo de flujos.
            // =========================================================
            SimulateWinMinigame();
        }
        else
        {
            // REGLA CORREGIDA: Herramienta incorrecta NO da strikes, ejecuta el feedback de rechazo
            if (wrongToolFeedbackCoroutine != null) StopCoroutine(wrongToolFeedbackCoroutine);
            wrongToolFeedbackCoroutine = StartCoroutine(WrongToolFeedbackRoutine());
        }
    }

    private IEnumerator WrongToolFeedbackRoutine()
    {
        if (sfxSource != null && rejectionSFX != null) sfxSource.PlayOneShot(rejectionSFX);

        // Feedback visual: Tiñe el aparato de rojo por un instante corto
        applianceMainImage.color = new Color(1f, 0.4f, 0.4f, 1f);

        if (gellyIntroChatBubble != null)
        {
            gellyIntroChatBubble.SetActive(true);
            gellyIntroTextMesh.text = "No creo que sea la herramienta correcta...";
        }

        yield return new WaitForSeconds(1.5f);

        applianceMainImage.color = Color.white;
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);
    }

    public void SimulateWinMinigame()
    {
        if (currentTaskIndex < taskTextMeshes.Length)
        {
            taskTextMeshes[currentTaskIndex].text = $"<s>{currentData.tasks[currentTaskIndex]}</s>";
        }

        currentTaskIndex++;
        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    /// <summary>
    /// LLÁMAME ÚNICAMENTE cuando el jugador pierda de verdad dentro del minijuego (En la sig. página)
    /// </summary>
    public void RegisterMinigameStrikeFailure()
    {
        LEGameSessionData.Instance.globalStrikes++;
        UpdateStrikesUI();

        if (LEGameSessionData.Instance.globalStrikes >= 3)
        {
            isMatchActive = false;
            SceneManager.LoadScene("LEGameOverScene");
            return;
        }

        currentState = RepairState.GameplayActive;
        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    private void EvaluateApplianceFixConclusion()
    {
        LEGameSessionData.Instance.repairedCount++;
        LEGameSessionData.Instance.currentMatchDataIndex++;

        int totalN = LEGameSessionData.Instance.totalSpawnedLimit - LEGameSessionData.Instance.discardedCount;

        if (LEGameSessionData.Instance.repairedCount >= totalN)
        {
            SceneManager.LoadScene("LEVictoryScene");
        }
        else
        {
            SceneManager.LoadScene("LEStartScene");
        }
    }

    private void HandleRemainingGlobalTimer()
    {
        LEGameSessionData.Instance.remainingTime -= Time.deltaTime;
        if (LEGameSessionData.Instance.remainingTime <= 0f)
        {
            LEGameSessionData.Instance.remainingTime = 0f;
            isMatchActive = false;
            SceneManager.LoadScene("LEGameOverScene");
            return;
        }

        int minutes = Mathf.FloorToInt(LEGameSessionData.Instance.remainingTime / 60f);
        int seconds = Mathf.FloorToInt(LEGameSessionData.Instance.remainingTime % 60f);

        if (minutes != cachedMinutes || seconds != cachedSeconds)
        {
            cachedMinutes = minutes;
            cachedSeconds = seconds;
            if (globalTimerTextMesh != null) globalTimerTextMesh.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateStrikesUI()
    {
        if (strikesTextMesh != null) strikesTextMesh.text = $"STRIKES: {LEGameSessionData.Instance.globalStrikes} / 3";
    }

    public void PauseGame()
    {
        if (currentState == RepairState.Paused) return;
        stateBeforePause = currentState;
        currentState = RepairState.Paused;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (currentState != RepairState.Paused) return;
        currentState = stateBeforePause;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
