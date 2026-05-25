using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class LETutorialManager : MonoBehaviour
{
    [System.Serializable]
    public struct TutorialStep
    {
        [TextArea(2, 4)] public string dialogueText; 
        public float delayBeforeStep;                 
        public float delayBeforeText; 
        public AudioClip stepAudio;                   
        public AudioClip voiceSound;  
        
        [Header("Character Action")]
        public bool moveCharacter;
        public Vector2 targetCharacterPosition;
        public float characterTargetScale; 
        public bool gellyFollowsChatPace; 

        [Header("Chat Bubble Movement")]
        public bool moveChatBubble;       
        public Vector2 targetChatAnchoredPosition; 
        public float chatBubbleDuration;      
        public float delayBeforeChatBubble;  

        [Header("Focus UI Action")]
        public bool useFocusUI;
        public RectTransform targetUIElement;
        [Range(0f, 0.5f)] public float focusCornerRadius;

        [Header("Custom Behavior (⌐■_■)")]
        [Tooltip("Cualquier función que coloques aquí se ejecutará inmediatamente al iniciar este paso.")]
        public UnityEvent onStepStartCustomAction; 
    }

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController;
    [SerializeField] private LETutorialFocusController focusController; 

    [Header("UI Chat Container Settings")]
    [SerializeField] private RectTransform chatBubbleContainer; 
    [SerializeField] private AnimationCurve chatBubbleCurve;    
    [SerializeField] private float defaultChatBubbleDuration = 0.5f; 
    [Tooltip("El objeto de la flecha que indica que el texto terminó y se puede continuar.")]
    [SerializeField] private GameObject continueArrowIndicator; 

    [Header("UI Text & Audio Elements")]
    [SerializeField] private TextMeshProUGUI dialogueTextMesh; 
    [SerializeField] private AudioSource audioSource;       
    [SerializeField] private AudioSource voiceAudioSource;  
    [SerializeField] private float textSpeed = 0.03f; 
    
    [Header("Retro Voice Settings")]
    [SerializeField] private int characterSoundInterval = 2; 
    [SerializeField] private bool randomizeVoicePitch = true; 

    [Header("Steps Configuration")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    [Header("End Tutorial Configuration (⌐■_■)")]
    [SerializeField] private GameObject tutorialPanel;
    [Tooltip("Posición final exacta en coordenadas del mundo a la que saltará Gelly al terminar el tutorial.")]
    [SerializeField] private Vector2 endTutorialTargetPosition; // <--- CONFIGURACIÓN EN TEXTO ABSOLUTO
    [Tooltip("Escala final que adoptará el personaje al aterrizar del salto de despedida.")]
    [SerializeField] private float endTutorialCharacterTargetScale = 1.0f; // <--- CONFIGURACIÓN EN TEXTO ABSOLUTO

    private int currentStepIndex = -1;
    private bool isActionExecuting = false; 
    private bool isTyping = false;          
    private Coroutine tutorialRoutine;
    private Coroutine typewriterCoroutine;
    private Coroutine chatBubbleCoroutine;
    private int currentPage = 1;
    private int totalPages = 1;

    public void StartTutorial()
    {
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        AdvanceTutorial();
    }

    public void AdvanceTutorial()
    {
        if (isTyping)
        {
            FinishCurrentPageInstantaneously();
            return;
        } 

        if (isActionExecuting) return; 

        if (currentPage < totalPages)
        {
            currentPage++;
            StartCoroutine(ExecutePageVisuals());
            return;
        }

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        if (tutorialRoutine != null) StopCoroutine(tutorialRoutine);
        tutorialRoutine = StartCoroutine(ExecuteStepRoutine(tutorialSteps[currentStepIndex]));
    }

    private IEnumerator ExecuteStepRoutine(TutorialStep step)
    {
        isActionExecuting = true; 
        currentPage = 1; 
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);

        step.onStepStartCustomAction?.Invoke();

        if (step.delayBeforeStep > 0f) yield return new WaitForSeconds(step.delayBeforeStep);
        if (audioSource != null && step.stepAudio != null) audioSource.PlayOneShot(step.stepAudio);

        if (dialogueTextMesh != null)
        {
            dialogueTextMesh.maxVisibleCharacters = 0; 
            dialogueTextMesh.text = step.dialogueText;
            dialogueTextMesh.pageToDisplay = currentPage;
            
            dialogueTextMesh.ForceMeshUpdate();
            totalPages = dialogueTextMesh.textInfo.pageCount;

            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.voiceSound, step.delayBeforeText));
        }

        float currentBubbleDuration = step.chatBubbleDuration <= 0f ? defaultChatBubbleDuration : step.chatBubbleDuration;

        if (step.moveChatBubble && chatBubbleContainer != null)
        {
            if (chatBubbleCoroutine != null) StopCoroutine(chatBubbleCoroutine);
            chatBubbleCoroutine = StartCoroutine(AnimateChatBubbleRoutine(step.targetChatAnchoredPosition, currentBubbleDuration, step.delayBeforeChatBubble));
        }

        if (step.moveCharacter)
        {
            float targetScale = step.characterTargetScale <= 0f ? 1f : step.characterTargetScale;

            if (step.gellyFollowsChatPace)
            {
                yield return StartCoroutine(DelayedGellyLinearMovement(step.targetCharacterPosition, targetScale, currentBubbleDuration, step.delayBeforeChatBubble, step));
            }
            else
            {
                bool characterMoving = true;
                gellyController.JumpTo(step.targetCharacterPosition, targetScale, () => 
                {
                    TriggerUIFocus(step);
                    characterMoving = false;
                });

                while (characterMoving) yield return null;
            }
            isActionExecuting = false;
        }
        else
        {
            if (step.moveChatBubble) yield return new WaitForSeconds(step.delayBeforeChatBubble + currentBubbleDuration);
            TriggerUIFocus(step);
            isActionExecuting = false;
        }
    }

    private IEnumerator ExecutePageVisuals()
    {
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        dialogueTextMesh.maxVisibleCharacters = dialogueTextMesh.textInfo.pageInfo[currentPage - 1].firstCharacterIndex;
        
        var step = tutorialSteps[currentStepIndex];
        typewriterCoroutine = StartCoroutine(TypewriterEffect(step.voiceSound, 0f));
        
        yield return null;
    }

    private IEnumerator TypewriterEffect(AudioClip voiceClip, float textDelay)
    {
        isTyping = true;
        dialogueTextMesh.pageToDisplay = currentPage;

        TMP_PageInfo pageInfo = dialogueTextMesh.textInfo.pageInfo[currentPage - 1];
        int firstCharacterIndex = pageInfo.firstCharacterIndex;
        int lastCharacterIndex = pageInfo.lastCharacterIndex;

        int counter = firstCharacterIndex;
        dialogueTextMesh.maxVisibleCharacters = counter;

        if (textDelay > 0f) yield return new WaitForSeconds(textDelay);

        while (counter <= lastCharacterIndex)
        {
            dialogueTextMesh.maxVisibleCharacters = counter;

            if (voiceClip != null && voiceAudioSource != null && counter > firstCharacterIndex)
            {
                char lastChar = dialogueTextMesh.text[counter - 1];
                if (counter % characterSoundInterval == 0 && !char.IsWhiteSpace(lastChar))
                {
                    if (randomizeVoicePitch) voiceAudioSource.pitch = Random.Range(0.93f, 1.07f);
                    voiceAudioSource.PlayOneShot(voiceClip);
                }
            }

            counter++;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false; 
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(true);
    }

    private void FinishCurrentPageInstantaneously()
    {
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        TMP_PageInfo pageInfo = dialogueTextMesh.textInfo.pageInfo[currentPage - 1];
        dialogueTextMesh.maxVisibleCharacters = pageInfo.lastCharacterIndex;
        
        isTyping = false; 
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(true);
    }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        EndTutorial();
        isTyping = false;
        isActionExecuting = false;
        Debug.Log("Tutorial salteado por el usuario con éxito. (o^^)o");
    }

    /// <summary>
    /// Se ejecuta al consumir el último paso del tutorial.
    /// </summary>
    private void EndTutorial()
    {
        focusController.HideFocus();
        if (chatBubbleContainer != null) chatBubbleContainer.gameObject.SetActive(false);
        if (dialogueTextMesh != null) dialogueTextMesh.text = "";

        // CINEMÁTICA FINAL CONFIGURABLE (⌐■_■)
        if (gellyController != null)
        {
            // Ejecuta el salto parabólico unificado hacia el destino absoluto fijado en el Inspector
            gellyController.JumpTo(endTutorialTargetPosition, endTutorialCharacterTargetScale, () => 
            {
                // =========================================================
                // TRANSICIÓN REAL: Aquí gatillas la carga de escena o nivel real
                // =========================================================
                tutorialPanel.SetActive(false);
            });
        }
    }

    private IEnumerator AnimateChatBubbleRoutine(Vector2 targetAnchoredPosition, float customDuration, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        Vector2 startAnchoredPos = chatBubbleContainer.anchoredPosition;
        float timer = 0f;
        while (timer < customDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / customDuration);
            float progress = chatBubbleCurve.Evaluate(t);
            chatBubbleContainer.anchoredPosition = Vector2.LerpUnclamped(startAnchoredPos, targetAnchoredPosition, progress);
            yield return null;
        }
        chatBubbleContainer.anchoredPosition = targetAnchoredPosition;
    }

    private IEnumerator DelayedGellyLinearMovement(Vector2 targetPos, float targetScale, float customDuration, float delay, TutorialStep step)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        bool linearMoveActive = true;
        gellyController.MoveLinearTo(targetPos, targetScale, customDuration, chatBubbleCurve, () => 
        {
            TriggerUIFocus(step);
            linearMoveActive = false;
        });

        while (linearMoveActive) yield return null;
    }

    private void TriggerUIFocus(TutorialStep step)
    {
        if (step.useFocusUI && step.targetUIElement != null) focusController.FocusOnElement(step.targetUIElement, step.focusCornerRadius);
        else focusController.HideFocus();
    }
}
