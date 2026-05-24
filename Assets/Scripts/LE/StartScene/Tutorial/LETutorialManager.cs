using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events; // Requerido para los Custom Behaviors en el Inspector

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
        public UnityEvent onStepStartCustomAction; // <--- SÚPER PODER PARA EVENTOS CUSTOM
    }

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController;
    [SerializeField] private LETutorialFocusController focusController; 

    [Header("UI Chat Container Settings")]
    [SerializeField] private RectTransform chatBubbleContainer; 
    [SerializeField] private AnimationCurve chatBubbleCurve;    
    [SerializeField] private float defaultChatBubbleDuration = 0.5f; 
    [Tooltip("El objeto de la flecha que indica que el texto terminó y se puede continuar.")]
    [SerializeField] private GameObject continueArrowIndicator; // <--- NUEVA REFERENCIA

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

    private int currentStepIndex = -1;
    private bool isStepExecuting = false;
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
        // TRUCO DE USABILIDAD: Si el texto aún se está escribiendo y el usuario hace click,
        // saltamos la animación para mostrar la página completa inmediatamente.
        if (isStepExecuting)
        {
            FinishCurrentPageInstantaneously();
            return;
        } 

        // Si hay más páginas por ver en este mismo paso, avanzamos la página en lugar del paso
        if (currentPage < totalPages)
        {
            currentPage++;
            StartCoroutine(ExecutePageVisuals());
            return;
        }

        // Si ya no quedan páginas, avanzamos al siguiente paso del tutorial normalmente
        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        StartCoroutine(ExecuteStepRoutine(tutorialSteps[currentStepIndex]));
    }

    private IEnumerator ExecuteStepRoutine(TutorialStep step)
    {
        isStepExecuting = true;
        currentPage = 1; 
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);

        step.onStepStartCustomAction?.Invoke();

        if (step.delayBeforeStep > 0f) yield return new WaitForSeconds(step.delayBeforeStep);
        if (audioSource != null && step.stepAudio != null) audioSource.PlayOneShot(step.stepAudio);

        if (dialogueTextMesh != null)
        {
            // ====================================================================
            // EL TRUCO MAESTRO: Bloqueamos la visibilidad en 0 ANTES de mutar el texto
            // ====================================================================
            dialogueTextMesh.maxVisibleCharacters = 0; 
            dialogueTextMesh.text = step.dialogueText;
            dialogueTextMesh.pageToDisplay = currentPage;
            
            // El cálculo geométrico ocurre aquí, pero como maxVisible es 0, la GPU no dibuja nada
            dialogueTextMesh.ForceMeshUpdate();
            totalPages = dialogueTextMesh.textInfo.pageCount;

            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.voiceSound, step.delayBeforeText));
        }

        // --- El resto del movimiento de la burbuja y Gelly se mantiene exactamente igual ---
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
                StartCoroutine(DelayedGellyLinearMovement(step.targetCharacterPosition, targetScale, currentBubbleDuration, step.delayBeforeChatBubble, step));
            }
            else
            {
                gellyController.JumpTo(step.targetCharacterPosition, targetScale, () => 
                {
                    TriggerUIFocus(step);
                    isStepExecuting = false; 
                });
            }
        }
        else
        {
            if (step.moveChatBubble) yield return new WaitForSeconds(step.delayBeforeChatBubble + currentBubbleDuration);
            TriggerUIFocus(step);
            isStepExecuting = false;
        }
    }

    private IEnumerator ExecutePageVisuals()
    {
        isStepExecuting = true;
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        // Al cambiar de página, también forzamos el bloqueo visual antes de iniciar la animación
        dialogueTextMesh.maxVisibleCharacters = dialogueTextMesh.textInfo.pageInfo[currentPage - 1].firstCharacterIndex;
        
        var step = tutorialSteps[currentStepIndex];
        typewriterCoroutine = StartCoroutine(TypewriterEffect(step.voiceSound, 0f));
        
        yield return null;
    }

    private IEnumerator TypewriterEffect(AudioClip voiceClip, float textDelay)
    {
        // Fijamos la página correcta
        dialogueTextMesh.pageToDisplay = currentPage;

        // Extraemos los índices puros de la página actual recalculada
        TMP_PageInfo pageInfo = dialogueTextMesh.textInfo.pageInfo[currentPage - 1];
        int firstCharacterIndex = pageInfo.firstCharacterIndex;
        int lastCharacterIndex = pageInfo.lastCharacterIndex;

        // Inicializamos el contenedor en el primer carácter exacto de esta página
        int counter = firstCharacterIndex;
        dialogueTextMesh.maxVisibleCharacters = counter;

        // Si hay un delay de texto personalizable, la pantalla se queda limpia y vacía durante la espera
        if (textDelay > 0f) yield return new WaitForSeconds(textDelay);

        while (counter <= lastCharacterIndex)
        {
            dialogueTextMesh.maxVisibleCharacters = counter;

            // Lógica de Audio/Voz tipo Undertale
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

        isStepExecuting = false;
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(true);
    }

    private void FinishCurrentPageInstantaneously()
    {
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        // Forzamos a TMP a mostrar todos los caracteres de la página actual de golpe
        TMP_PageInfo pageInfo = dialogueTextMesh.textInfo.pageInfo[currentPage - 1];
        dialogueTextMesh.maxVisibleCharacters = pageInfo.lastCharacterIndex;
        
        isStepExecuting = false;
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(true);
    }

    /// <summary>
    /// ¡ESTA FUNCIÓN VA EN TU BOTÓN DE REVENTAR/SALTAR TUTORIAL! 💥
    /// Detiene todo en seco de forma segura y limpia la pantalla.
    /// </summary>
    public void SkipTutorial()
    {
        // 1. Detenemos todas las corrutinas activas para evitar desfases de tiempo
        StopAllCoroutines();
        
        // 2. Apagamos los indicadores visuales flotantes
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        
        // 3. Forzamos el cierre del sistema
        EndTutorial();
        
        // 4. Liberamos el flag por seguridad
        isStepExecuting = false;
        
        Debug.Log("Tutorial salteado por el usuario con éxito. (o^^)o");
    }

    private void EndTutorial()
    {
        focusController.HideFocus();
        // Ocultamos o desactivamos el contenedor completo de la burbuja de chat si así lo deseas
        if (chatBubbleContainer != null) chatBubbleContainer.gameObject.SetActive(false);
        if (dialogueTextMesh != null) dialogueTextMesh.text = "";
    }

    // --- Métodos de soporte de animaciones se mantienen idénticos ---
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
        gellyController.MoveLinearTo(targetPos, targetScale, customDuration, chatBubbleCurve, () => 
        {
            TriggerUIFocus(step);
            isStepExecuting = false; 
        });
    }

    private void TriggerUIFocus(TutorialStep step)
    {
        if (step.useFocusUI && step.targetUIElement != null) focusController.FocusOnElement(step.targetUIElement, step.focusCornerRadius);
        else focusController.HideFocus();
    }
}
