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
        [Tooltip("Delay específico para que el texto comience a escribirse tras iniciar el paso.")]
        public float delayBeforeText; // <--- NUEVA VARIABLE
        public AudioClip stepAudio;                   
        [Tooltip("El sonido corto (blip/click) que sonará al escribir estilo Undertale.")]
        public AudioClip voiceSound;  // <--- NUEVA VARIABLE
        
        [Header("Character Action")]
        public bool moveCharacter;
        public Vector2 targetCharacterPosition;
        [Tooltip("El tamaño/escala general que adoptará el personaje en este paso.")]
        public float characterTargetScale; 

        [Header("Focus UI Action")]
        public bool useFocusUI;
        public RectTransform targetUIElement;
        [Range(0f, 0.5f)] public float focusCornerRadius;
    }

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController;
    [SerializeField] private LETutorialFocusController focusController; 

    [Header("UI & Audio Elements")]
    [SerializeField] private TextMeshProUGUI dialogueTextMesh; 
    [SerializeField] private AudioSource audioSource;       // FX General (Cambio de paso)
    [SerializeField] private AudioSource voiceAudioSource;  // <--- NUEVA REFERENCIA: Canal exclusivo para los blips de voz
    [SerializeField] private float textSpeed = 0.03f; 
    
    [Header("Retro Voice Settings")]
    [Tooltip("Cada cuántos caracteres se reproducirá el sonido. 2 o 3 evita saturación.")]
    [SerializeField] private int characterSoundInterval = 2; 
    [Tooltip("Varía ligeramente el tono de cada letra para un efecto más orgánico y vivo.")]
    [SerializeField] private bool randomizeVoicePitch = true; 

    [Header("Steps Configuration")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStepIndex = -1;
    private bool isStepExecuting = false;
    private Coroutine typewriterCoroutine;

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

        // 1. Aplicar el Delay del Paso si existe
        if (step.delayBeforeStep > 0f)
        {
            yield return new WaitForSeconds(step.delayBeforeStep);
        }

        // 2. FX de cambio de paso
        if (audioSource != null && step.stepAudio != null)
        {
            audioSource.PlayOneShot(step.stepAudio);
        }

        // 3. Iniciar animación de escritura pasándole sus nuevos parámetros de audio y delay de texto
        if (dialogueTextMesh != null)
        {
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.dialogueText, step.voiceSound, step.delayBeforeText));
        }

        // 4. Resolver movimiento del personaje
        if (step.moveCharacter)
        {
            float targetScale = step.characterTargetScale <= 0f ? 1f : step.characterTargetScale;

            gellyController.JumpTo(step.targetCharacterPosition, targetScale, () => 
            {
                TriggerUIFocus(step);
                isStepExecuting = false; 
            });
        }
        else
        {
            TriggerUIFocus(step);
            isStepExecuting = false;
        }
    }

    private IEnumerator TypewriterEffect(string fullText, AudioClip voiceClip, float textDelay)
    {
        dialogueTextMesh.text = fullText;
        dialogueTextMesh.maxVisibleCharacters = 0;
        
        // Aplicamos el delay personalizado antes de que la primera letra aparezca
        if (textDelay > 0f)
        {
            yield return new WaitForSeconds(textDelay);
        }

        yield return null; 

        int totalCharacters = fullText.Length;
        int counter = 0;

        while (counter <= totalCharacters)
        {
            dialogueTextMesh.maxVisibleCharacters = counter;

            // =========================================================
            // LÓGICA DE VOZ RETRO PREMIUM STYLE (⌐■_■)
            // =========================================================
            if (voiceClip != null && voiceAudioSource != null && counter > 0 && counter < totalCharacters)
            {
                char lastChar = fullText[counter - 1];

                // Regla de Oro: Solo suena en el intervalo de caracteres configurado Y si NO es un espacio vacío
                if (counter % characterSoundInterval == 0 && !char.IsWhiteSpace(lastChar))
                {
                    if (randomizeVoicePitch)
                    {
                        // Modulación de frecuencia elástica sutil
                        voiceAudioSource.pitch = Random.Range(0.93f, 1.07f); 
                    }
                    
                    voiceAudioSource.PlayOneShot(voiceClip);
                }
            }
            // =========================================================

            counter++;
            yield return new WaitForSeconds(textSpeed);
        }

        // Al terminar la escritura, restablecemos el pitch a su estado neutral por seguridad
        if (voiceAudioSource != null) voiceAudioSource.pitch = 1f;
    }

    private void TriggerUIFocus(TutorialStep step)
    {
        if (step.useFocusUI && step.targetUIElement != null)
        {
            focusController.FocusOnElement(step.targetUIElement, step.focusCornerRadius);
        }
        else
        {
            focusController.HideFocus();
        }
    }

    private void EndTutorial()
    {
        focusController.HideFocus();
        if (dialogueTextMesh != null) dialogueTextMesh.text = "¡Tutorial finalizado! (^_^)";
    }
}
