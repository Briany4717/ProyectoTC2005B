using UnityEngine;
using TMPro;
using System.Collections;

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
        [Tooltip("Si está activo, Gelly se moverá de forma plana siguiendo el mismo delay, duración y curva elástica que la burbuja.")]
        public bool gellyFollowsChatPace; 

        [Header("Chat Bubble Movement")]
        public bool moveChatBubble;       
        public Vector2 targetChatAnchoredPosition; 
        [Tooltip("Duración personalizada para el viaje de la burbuja en este paso. Si es 0, usa la duración por defecto.")]
        public float chatBubbleDuration;      // <--- NUEVA VARIABLE
        [Tooltip("Retraso específico antes de que la burbuja comience a moverse en este paso.")]
        public float delayBeforeChatBubble;  // <--- NUEVA VARIABLE

        [Header("Focus UI Action")]
        public bool useFocusUI;
        public RectTransform targetUIElement;
        [Range(0f, 0.5f)] public float focusCornerRadius;
    }

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController;
    [SerializeField] private LETutorialFocusController focusController; 

    [Header("UI Chat Container Settings")]
    [SerializeField] private RectTransform chatBubbleContainer; 
    [SerializeField] private AnimationCurve chatBubbleCurve;    
    [Tooltip("Duración global por defecto en caso de que un paso tenga Chat Bubble Duration en 0.")]
    [SerializeField] private float defaultChatBubbleDuration = 0.5f; // Fallback seguro

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
    private Coroutine typewriterCoroutine;
    private Coroutine chatBubbleCoroutine;

    public void StartTutorial()
    {
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

        StartCoroutine(ExecuteStepRoutine(tutorialSteps[currentStepIndex]));
    }

    private IEnumerator ExecuteStepRoutine(TutorialStep step)
    {
        isStepExecuting = true;

        // 1. Delay inicial del paso general
        if (step.delayBeforeStep > 0f)
        {
            yield return new WaitForSeconds(step.delayBeforeStep);
        }

        // 2. FX e inicio de escritura de texto
        if (audioSource != null && step.stepAudio != null) audioSource.PlayOneShot(step.stepAudio);

        if (dialogueTextMesh != null)
        {
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.dialogueText, step.voiceSound, step.delayBeforeText));
        }

        // Calcular la duración real de la burbuja para este frame (usar la del paso o el fallback global)
        float currentBubbleDuration = step.chatBubbleDuration <= 0f ? defaultChatBubbleDuration : step.chatBubbleDuration;

        // 3. Mover el contenedor de diálogos con su propio Delay y Duración personalizados
        if (step.moveChatBubble && chatBubbleContainer != null)
        {
            if (chatBubbleCoroutine != null) StopCoroutine(chatBubbleCoroutine);
            chatBubbleCoroutine = StartCoroutine(AnimateChatBubbleRoutine(step.targetChatAnchoredPosition, currentBubbleDuration, step.delayBeforeChatBubble));
        }

        // 4. Resolver movimiento de Gelly (Salto o Deslizamiento Sincronizado Completo)
        if (step.moveCharacter)
        {
            float targetScale = step.characterTargetScale <= 0f ? 1f : step.characterTargetScale;

            if (step.gellyFollowsChatPace)
            {
                // MÁXIMA SINCRONIZACIÓN: Gelly espera el mismo delay y corre a la misma velocidad que la burbuja
                StartCoroutine(DelayedGellyLinearMovement(step.targetCharacterPosition, targetScale, currentBubbleDuration, step.delayBeforeChatBubble, step));
            }
            else
            {
                // Salto parabólico estándar independiente (ignora los tiempos de la burbuja)
                gellyController.JumpTo(step.targetCharacterPosition, targetScale, () => 
                {
                    TriggerUIFocus(step);
                    isStepExecuting = false; 
                });
            }
        }
        else
        {
            // Si Gelly no se mueve, esperamos a que la burbuja termine su viaje completo (Delay + Duración) antes de liberar el paso
            if (step.moveChatBubble)
            {
                yield return new WaitForSeconds(step.delayBeforeChatBubble + currentBubbleDuration);
            }
            
            TriggerUIFocus(step);
            isStepExecuting = false;
        }
    }

    /// <summary>
    /// Corrutina que traslada la burbuja aplicando el delay y duración específicos del paso actual
    /// </summary>
    private IEnumerator AnimateChatBubbleRoutine(Vector2 targetAnchoredPosition, float customDuration, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

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

    /// <summary>
    /// Sincronizador secundario: Hace que Gelly espere el delay de la burbuja y se desplace en perfecta armonía lineal
    /// </summary>
    private IEnumerator DelayedGellyLinearMovement(Vector2 targetPos, float targetScale, float customDuration, float delay, TutorialStep step)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        gellyController.MoveLinearTo(targetPos, targetScale, customDuration, chatBubbleCurve, () => 
        {
            TriggerUIFocus(step);
            isStepExecuting = false; 
        });
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
    }

    private void TriggerUIFocus(TutorialStep step)
    {
        if (step.useFocusUI && step.targetUIElement != null) focusController.FocusOnElement(step.targetUIElement, step.focusCornerRadius);
        else focusController.HideFocus();
    }

    private void EndTutorial()
    {
        focusController.HideFocus();
        if (dialogueTextMesh != null) dialogueTextMesh.text = "¡Tutorial finalizado! (^_^)";
    }
}
