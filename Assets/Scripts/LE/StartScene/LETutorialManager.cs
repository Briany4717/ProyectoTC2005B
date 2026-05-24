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
        public AudioClip stepAudio;                   
        
        [Header("Character Action")]
        public bool moveCharacter;
        public Vector2 targetCharacterPosition;
        [Tooltip("El tamaño/escala general que adoptará el personaje en este paso.")]
        public float characterTargetScale; // <--- NUEVA VARIABLE

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
    [SerializeField] private AudioSource audioSource; // Componente que reproducirá tus clips
    [SerializeField] private float textSpeed = 0.03f; // Tiempo entre cada letra

    [Header("Steps Configuration")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStepIndex = -1;
    private bool isStepExecuting = false;
    private Coroutine typewriterCoroutine;

    public void StartTutorial()
    {
        // Posición inicial base
        AdvanceTutorial();
    }

    /// <summary>
    /// Vincula esta función directamente al OnClick() de tus botones en Unity.
    /// </summary>
    public void AdvanceTutorial()
    {
        if (isStepExecuting) return; 

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        // Iniciamos el proceso mediante una Corrutina para poder manejar el Delay limpiamente
        StartCoroutine(ExecuteStepRoutine(tutorialSteps[currentStepIndex]));
    }

    private IEnumerator ExecuteStepRoutine(TutorialStep step)
    {
        isStepExecuting = true;

        // 1. Aplicar el Delay Configurable si existe
        if (step.delayBeforeStep > 0f)
        {
            yield return new WaitForSeconds(step.delayBeforeStep);
        }

        if (audioSource != null && step.stepAudio != null)
        {
            audioSource.PlayOneShot(step.stepAudio);
        }

        if (dialogueTextMesh != null)
        {
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(step.dialogueText));
        }

        // 4. Resolver movimiento del personaje
        if (step.moveCharacter)
        {
            // Si la escala viene en 0 por descuido en el inspector, la forzamos a 1 para que no sea invisible
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

    private IEnumerator TypewriterEffect(string fullText)
    {
        dialogueTextMesh.text = fullText;
        dialogueTextMesh.maxVisibleCharacters = 0;
        
        // Esperamos un frame para que TextMeshPro genere internamente los caracteres
        yield return null; 

        int totalCharacters = fullText.Length;
        int counter = 0;

        while (counter <= totalCharacters)
        {
            dialogueTextMesh.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(textSpeed);
        }
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
