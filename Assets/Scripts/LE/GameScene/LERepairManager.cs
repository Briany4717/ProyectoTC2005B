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

    [Header("Gelly Intro Chat Bubble & Typewriter (⌐■_■)")]
    [SerializeField] private GameObject gellyIntroChatBubble;
    [SerializeField] private TextMeshProUGUI gellyIntroTextMesh;
    [Tooltip("Velocidad de escritura de la intro (Segundos por letra).")]
    [SerializeField] private float introTextSpeed = 0.03f;
    [Tooltip("Cada cuántas letras suena la voz de Gelly.")]
    [SerializeField] private int voiceSoundInterval = 2;
    [Tooltip("Tiempo que se queda visible la burbuja de inicio DESPUÉS de terminar de escribirse.")]
    [SerializeField] private float introDisplayDuration = 2.0f; // <--- CONFIGURABLE POR TIEMPO
    [Tooltip("Tiempo que se queda visible la burbuja de error de herramienta.")]
    [SerializeField] private float wrongToolDisplayDuration = 1.8f; // <--- CONFIGURABLE POR TIEMPO

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
    [SerializeField] private TextMeshProUGUI[] taskTextMeshes; 

    [Header("Strikes UI")]
    [SerializeField] private TextMeshProUGUI strikesTextMesh;

    [Header("Procedural Debris & Animation Settings")]
    [Tooltip("Duración exacta en segundos del sacudido del electrodoméstico.")]
    [SerializeField] private float repairAnimDuration = 2.5f;
    [SerializeField] private float shakeSpeed = 80f;
    [SerializeField] private float shakeIntensity = 7.0f;
    [SerializeField] private GameObject debrisUIPrefab; 
    [SerializeField] private Transform debrisSpawnPoint;
    [SerializeField] private Sprite[] debrisSpritesPool; 
    [SerializeField] private float timeBetweenDebrisSpawns = 0.08f;

    [Header("Audio Source & Retro Clips")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip gellyVoiceSound;
    [SerializeField] private AudioClip rejectionSFX;

    private LEApplianceRepairData currentData;
    private Vector3 applianceOriginalPosition;
    private Vector3 problemBubbleOriginalAnchoredPos;
    
    private int currentTaskIndex = 0;
    private bool isMatchActive = false;
    
    private int cachedMinutes = -1;
    private int cachedSeconds = -1;
    private RepairState stateBeforePause;
    
    private Coroutine introFlowCoroutine;
    private Coroutine wrongToolFeedbackCoroutine;
    private string cachedIntroPhrase;

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
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(true);
        
        cachedIntroPhrase = Random.Range(0, 2) == 0 ? "¡Manos a la obra con esto!" : "¡Comencemos la reparación ya!";
        
        // Iniciamos el flujo automático controlado por tiempo (0 clicks requeridos)
        if (introFlowCoroutine != null) StopCoroutine(introFlowCoroutine);
        introFlowCoroutine = StartCoroutine(AutomatedIntroFlowRoutine());
    }

    /// <summary>
    /// Flujo de introducción controlado estrictamente por tiempo e inmune a pausas.
    /// </summary>
    private IEnumerator AutomatedIntroFlowRoutine()
    {
        gellyIntroTextMesh.text = cachedIntroPhrase;
        gellyIntroTextMesh.maxVisibleCharacters = 0;
        gellyIntroTextMesh.ForceMeshUpdate();

        int totalVisibleCharacters = cachedIntroPhrase.Length;
        int counter = 0;
        float typewriterTimer = 0f;

        // 1. Escritura animada de la Intro con soporte para pausa por código
        while (counter <= totalVisibleCharacters)
        {
            if (currentState != RepairState.Paused)
            {
                typewriterTimer += Time.deltaTime;
                if (typewriterTimer >= introTextSpeed)
                {
                    typewriterTimer = 0f;
                    gellyIntroTextMesh.maxVisibleCharacters = counter;

                    if (gellyVoiceSound != null && sfxAudioSource != null && counter > 0)
                    {
                        char lastChar = cachedIntroPhrase[counter - 1];
                        if (counter % voiceSoundInterval == 0 && !char.IsWhiteSpace(lastChar))
                        {
                            sfxAudioSource.pitch = Random.Range(0.94f, 1.06f);
                            sfxAudioSource.PlayOneShot(gellyVoiceSound);
                        }
                    }
                    counter++;
                }
            }
            yield return null;
        }

        // 2. Espera estricta configurable post-escritura (Soporta congelación en pausa)
        float displayTimer = 0f;
        while (displayTimer < introDisplayDuration)
        {
            if (currentState != RepairState.Paused)
            {
                displayTimer += Time.deltaTime;
            }
            yield return null;
        }

        // 3. Transición automática al sacudido de reparación
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);
        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    private IEnumerator ExecuteRepairAnimationRoutine()
    {
        currentState = RepairState.RepairingAnimation;
        
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

                float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
                applianceMainImage.transform.position = applianceOriginalPosition + new Vector3(shakeX, 0f, 0f);

                if (debrisTimer >= timeBetweenDebrisSpawns && debrisSpritesPool.Length > 0 && debrisUIPrefab != null)
                {
                    debrisTimer = 0f;
                    GameObject debrisObj = Instantiate(debrisUIPrefab, transform); 
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
        
        // REGLA: Al cerrar la problemática, el globo del "?" emerge de inmediato para re-consulta
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(true);
    }

    public void ProcessToolDropped(int toolId)
    {
        if (currentState != RepairState.GameplayActive) return;

        if (toolId == currentData.correctToolId)
        {
            if (wrongToolFeedbackCoroutine != null) StopCoroutine(wrongToolFeedbackCoroutine);
            applianceMainImage.color = Color.white;
            currentState = RepairState.ExecutingMinigame;
            
            // =========================================================
            // COPLAS DE MINIJUEGO: Listos para inyectar tu minijuego interactivo.
            // =========================================================
            SimulateWinMinigame();
        }
        else
        {
            // REGLA: Herramienta incorrecta gatilla feedback de RECHAZO automático por tiempo
            if (wrongToolFeedbackCoroutine != null) StopCoroutine(wrongToolFeedbackCoroutine);
            wrongToolFeedbackCoroutine = StartCoroutine(WrongToolFeedbackRoutine());
        }
    }

    private IEnumerator WrongToolFeedbackRoutine()
    {
        if (sfxAudioSource != null && rejectionSFX != null) sfxAudioSource.PlayOneShot(rejectionSFX);

        applianceMainImage.color = new Color(1f, 0.4f, 0.4f, 1f);

        if (gellyIntroChatBubble != null)
        {
            gellyIntroChatBubble.SetActive(true);
            gellyIntroTextMesh.maxVisibleCharacters = 999; 
            gellyIntroTextMesh.text = "No creo que sea la herramienta correcta...";
        }

        // REGLA CONTROLADA POR TIEMPO E INMUNE A PAUSA (o^^)o
        float errorTimer = 0f;
        while (errorTimer < wrongToolDisplayDuration)
        {
            if (currentState != RepairState.Paused)
            {
                errorTimer += Time.deltaTime;
            }
            yield return null;
        }

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

    private void EvaluateApplianceFixConclusion()
    {
        LEGameSessionData.Instance.repairedCount++;
        LEGameSessionData.Instance.currentMatchDataIndex++;
        int totalN = LEGameSessionData.Instance.totalSpawnedLimit - LEGameSessionData.Instance.discardedCount;

        SceneManager.LoadScene(LEGameSessionData.Instance.repairedCount >= totalN ? "LEVictoryScene" : "LEConveyorScene");
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
