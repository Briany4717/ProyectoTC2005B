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

    public void StartTutorial()
    {
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);
        AdvanceTutorial();
    }

    public void AdvanceTutorial()
    {
        if (isStepExecuting) return; 

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        // Guardamos la rutina principal para poder detenerla limpiamente si se salta el tutorial
        if (tutorialRoutine != null) StopCoroutine(tutorialRoutine);
        tutorialRoutine = StartCoroutine(ExecuteStepRoutine(tutorialSteps[currentStepIndex]));
    }

    private IEnumerator ExecuteStepRoutine(TutorialStep step)
    {
        isStepExecuting = true;

        // Ocultamos la flecha de continuación al iniciar un nuevo paso
        if (continueArrowIndicator != null) continueArrowIndicator.SetActive(false);

        // =========================================================
        // EJECUCIÓN DE CUSTOM BEHAVIOR
        // =========================================================
        step.onStepStartCustomAction?.Invoke(); // Dispara lo que sea que hayas programado en el inspector
        // =========================================================

        if (step.delayBeforeStep > 0f)
        {
            yield return new WaitForSeconds(step.delayBeforeStep);
        }

        if (audioSource != null && step.stepAudio != null) audioSource.PlayOneShot(step.stepAudio);

        if (dialogueTextMesh != null)
        {
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.dialogueText, step.voiceSound, step.delayBeforeText));
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
            if (step.moveChatBubble)
            {
                yield return new WaitForSeconds(step.delayBeforeChatBubble + currentBubbleDuration);
            }
            
            TriggerUIFocus(step);
            isStepExecuting = false;
        }
    }

    private IEnumerator TypewriterEffect(string fullText, AudioClip voiceClip, float textDelay)
    {
        dialogueTextMesh.text = fullText;
        dialogueTextMesh.maxVisibleCharacters = 0;
        
        if (textDelay > 0f) yield return new WaitForSeconds(textDelay);

        yield return null; 

        int totalCharacters = fullText.Length;
        int counter = 0;

        while (counter <= totalCharacters)
        {
            dialogueTextMesh.maxVisibleCharacters = counter;

            if (voiceClip != null && voiceAudioSource != null && counter > 0 && counter < totalCharacters)
            {
                char lastChar = fullText[counter - 1];
                if (counter % characterSoundInterval == 0 && !char.IsWhiteSpace(lastChar))
                {
                    if (randomizeVoicePitch) voiceAudioSource.pitch = Random.Range(0.93f, 1.07f);
                    voiceAudioSource.PlayOneShot(voiceClip);
                }
            }

            counter++;
            yield return new WaitForSeconds(textSpeed);
        }

        if (voiceAudioSource != null) voiceAudioSource.pitch = 1f;

        // ¡MÁXIMA JUGOSIDAD!: Al terminar de escribir la última letra, encendemos la flecha indicadora
        if (continueArrowIndicator != null)
        {
            continueArrowIndicator.SetActive(true);
        }
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
