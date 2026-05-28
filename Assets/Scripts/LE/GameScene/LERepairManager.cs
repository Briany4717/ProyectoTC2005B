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

    [Header("Gelly Intro Chat Bubble & Typewriter")]
    [SerializeField] private GameObject gellyIntroChatBubble;
    [SerializeField] private TextMeshProUGUI gellyIntroTextMesh;
    [SerializeField] private float introTextSpeed = 0.03f;
    [SerializeField] private int voiceSoundInterval = 2;
    [SerializeField] private float introDisplayDuration = 2.0f; 
    [SerializeField] private float wrongToolDisplayDuration = 1.8f; 

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

    [Header("Strikes UI (Visual X System)")]
    [SerializeField] private GameObject[] strikeVisualElements; 

    [Header("Procedural Debris & Rotational Animation")]
    [SerializeField] private float repairAnimDuration = 2.5f;
    [SerializeField] private float shakeSpeed = 80f;
    [SerializeField] private float shakeIntensity = 8.0f; 
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
    private Quaternion applianceOriginalRotation; 
    private Vector3 problemBubbleOriginalAnchoredPos;
    
    private int currentTaskIndex = 0;
    private bool isMatchActive = false;
    
    private int cachedMinutes = -1;
    private int cachedSeconds = -1;
    private RepairState stateBeforePause;
    
    private Coroutine introFlowCoroutine;
    private Coroutine wrongToolFeedbackCoroutine;

    [Header("Minigames Container")]
    [SerializeField] private LETicTacToeMinigame ticTacToeMinigame;
    [SerializeField] private LEHanoiMinigame hanoiMinigame;
    [SerializeField] private LEFlappyMinigame flappyMinigame;
    [SerializeField] private GameObject[] instructionSteps;
    private int instructionStepIndex = 0;
    private int minigameIndex = 1;
    public bool presetationMode = true;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (problemDialogueOverlayPanel != null) problemDialogueOverlayPanel.SetActive(false);
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(false);
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(false);

        applianceOriginalPosition = applianceMainImage.transform.position;
        applianceOriginalRotation = applianceMainImage.transform.localRotation;

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
            if (i < currentData.steps.Length) 
                taskTextMeshes[i].text = currentData.steps[i].taskDescription;
        }

        UpdateStrikesUI();
        isMatchActive = true;
    }

    private void InitializeRepairFlow()
    {
        currentState = RepairState.IntroDialogue;
        if (gellyIntroChatBubble != null) gellyIntroChatBubble.SetActive(true);
        
        if (introFlowCoroutine != null) StopCoroutine(introFlowCoroutine);
        introFlowCoroutine = StartCoroutine(AutomatedIntroFlowRoutine());
    }

    private IEnumerator AutomatedIntroFlowRoutine()
    {
        string activeIntroText = "¡Hora de Trabajar!";

        gellyIntroTextMesh.text = activeIntroText;
        gellyIntroTextMesh.maxVisibleCharacters = 0;
        gellyIntroTextMesh.ForceMeshUpdate();

        int totalVisibleCharacters = activeIntroText.Length;
        int counter = 0;
        float typewriterTimer = 0f;

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
                        char lastChar = activeIntroText[counter - 1];
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

        float displayTimer = 0f;
        while (displayTimer < introDisplayDuration)
        {
            if (currentState != RepairState.Paused) displayTimer += Time.deltaTime;
            yield return null;
        }

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

                float rotZ = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
                applianceMainImage.transform.localRotation = applianceOriginalRotation * Quaternion.Euler(0f, 0f, rotZ);

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

        applianceMainImage.transform.localRotation = applianceOriginalRotation;

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

        if (overlayTitleText != null) overlayTitleText.text = currentData.steps[currentTaskIndex].gellyDialogue;
        if (overlayBodyText != null) overlayBodyText.text = currentData.steps[currentTaskIndex].problemText;
    }

    public void CloseProblemDialogueWindow()
    {
        if (currentState != RepairState.ProblemDialogue) return;
        if (problemDialogueOverlayPanel != null) problemDialogueOverlayPanel.SetActive(false);

        currentState = RepairState.GameplayActive;
        if (gellyProblemBubbleContainer != null) gellyProblemBubbleContainer.SetActive(true);
    }

    public void ProcessToolDropped(int toolId)
    {
        if (currentState != RepairState.GameplayActive) return;

        if (toolId == currentData.steps[currentTaskIndex].correctToolId)
        {
            if (wrongToolFeedbackCoroutine != null) StopCoroutine(wrongToolFeedbackCoroutine);
            applianceMainImage.color = Color.white;
            
            currentState = RepairState.ExecutingMinigame;
            
            int randomMinigameIndex = minigameIndex;
            minigameIndex++;
            if(minigameIndex > 3) minigameIndex = 1;
            if (!presetationMode)
            {
                randomMinigameIndex = Random.Range(1,3);
            }
            bool minigameLaunched = false;

            switch (randomMinigameIndex)
            {
                case 1:
                    if (ticTacToeMinigame != null) { ticTacToeMinigame.StartMinigame(); minigameLaunched = true; }
                    break;
                case 2:
                    if (hanoiMinigame != null) { hanoiMinigame.StartMinigame(); minigameLaunched = true; }
                    break;
                case 3:
                    if (flappyMinigame != null) { flappyMinigame.StartMinigame(); minigameLaunched = true; }
                    break;
            }

            if (!minigameLaunched)
            {
                if (ticTacToeMinigame != null) ticTacToeMinigame.StartMinigame();
                else if (hanoiMinigame != null) hanoiMinigame.StartMinigame();
                else if (flappyMinigame != null) flappyMinigame.StartMinigame();
                else
                {
                    SimulateWinMinigame();
                }
            }
        }
        else
        {
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
            gellyProblemBubbleContainer.SetActive(false);
            gellyIntroTextMesh.maxVisibleCharacters = 999; 
            gellyIntroTextMesh.text = "No creo que sea la herramienta correcta...";
        }

        float errorTimer = 0f;
        while (errorTimer < wrongToolDisplayDuration)
        {
            if (currentState != RepairState.Paused) errorTimer += Time.deltaTime;
            yield return null;
        }

        applianceMainImage.color = Color.white;
        if (gellyIntroChatBubble != null) 
        {
            gellyIntroChatBubble.SetActive(false);
            gellyProblemBubbleContainer.SetActive(true);
        }
    }

    public void SimulateWinMinigame()
    {
        if (currentTaskIndex < taskTextMeshes.Length)
        {
            taskTextMeshes[currentTaskIndex].text = $"<s>{currentData.steps[currentTaskIndex].taskDescription}</s>";
        }
        currentTaskIndex++;
        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    public void RegisterMinigameStrikeFailure()
    {
        LEGameSessionData.Instance.globalStrikes++;
        UpdateStrikesUI();

        if (LEGameSessionData.Instance.globalStrikes >= 3)
        {
            isMatchActive = false;
            LEGameSessionData.Instance.isVictory = false; 
            SceneManager.LoadScene("LEFinalScene"); 
            return;
        }

        StartCoroutine(ExecuteRepairAnimationRoutine());
    }

    private void EvaluateApplianceFixConclusion()
    {
        LEGameSessionData.Instance.repairedCount++;
        LEGameSessionData.Instance.currentMatchDataIndex++;
        int totalN = LEGameSessionData.Instance.totalSpawnedLimit - LEGameSessionData.Instance.discardedCount;
        LEGameSessionData.Instance.isVictory = LEGameSessionData.Instance.repairedCount >= totalN;
        SceneManager.LoadScene(LEGameSessionData.Instance.repairedCount >= totalN ? "LEFinalScene" : "LEConveyorScene");
    }

    private void HandleRemainingGlobalTimer()
    {
        LEGameSessionData.Instance.remainingTime -= Time.deltaTime;
        if (LEGameSessionData.Instance.remainingTime <= 0f)
        {
            LEGameSessionData.Instance.remainingTime = 0f;
            isMatchActive = false;
            SceneManager.LoadScene("LEFinalScene");
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
        if (strikeVisualElements == null || strikeVisualElements.Length == 0) return;
        int currentStrikes = LEGameSessionData.Instance.globalStrikes;
        for (int i = 0; i < strikeVisualElements.Length; i++)
        {
            if (strikeVisualElements[i] != null) strikeVisualElements[i].SetActive(i < currentStrikes);
        }
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

    public void ShowInstructions()
    {
        if (instructionSteps.Length > 0)
        {
            instructionStepIndex = 0;
            instructionSteps[0].SetActive(true);
        }
    }

    public void CloseInsructions()
    {
        if (instructionSteps.Length > 0)
        {
            instructionSteps[instructionStepIndex].SetActive(false);
        }
    }

    public void GoBackInstruction()
    {
        if (instructionStepIndex - 1 >= 0)
        {
            instructionSteps[instructionStepIndex].SetActive(false);
            instructionStepIndex--;
            instructionSteps[instructionStepIndex].SetActive(true);
        }
    }

    public void GoNextInstruction()
    {
        if (instructionStepIndex + 1 < instructionSteps.Length)
        {
            instructionSteps[instructionStepIndex].SetActive(false);
            instructionStepIndex++;
            instructionSteps[instructionStepIndex].SetActive(true);
        }
    }

    public void StopGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void GoToMenu()
    {
        LEGameSessionData.Instance.ResetSession(PlayerPrefs.GetFloat("LE_Minigame_Duration", 330f)); 
        SceneManager.LoadScene("MenuScene");
    }
}
