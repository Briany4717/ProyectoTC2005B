using UnityEngine;
using TMPro; // Para los textos premium

public class LETutorialManager : MonoBehaviour
{
    [System.Serializable]
    public struct TutorialStep
    {
        [TextArea(2, 4)] public string dialogueText; // El texto que se mostrará en este paso
        
        [Header("Character Action")]
        public bool moveCharacter;
        public Vector2 targetCharacterPosition;

        [Header("Focus UI Action")]
        public bool useFocusUI;
        public RectTransform targetUIElement;
        [Range(0f, 0.5f)] public float focusCornerRadius;
    }

    [Header("Controllers Shared")]
    [SerializeField] private LEGellyCharacterController gellyController;
    [SerializeField] private LETutorialFocusController focusController; // El script del shader que hicimos antes

    [Header("UI Fields")]
    [SerializeField] private TextMeshProUGUI dialogueTextMesh; // Tu componente de texto de la UI

    [Header("Steps Configuration")]
    [SerializeField] private TutorialStep[] tutorialSteps;

    private int currentStepIndex = -1;
    private bool isStepExecuting = false;

    public void StartTutorial()
    {
        // Arrancamos en el paso cero
        AdvanceTutorial();
    }

    /// <summary>
    /// ¡ESTA FUNCIÓN VA EN EL BOTÓN DE UNITY! 
    /// Cada vez que el usuario presione el botón "Siguiente", llamará aquí.
    /// </summary>
    public void AdvanceTutorial()
    {
        // Bloqueamos clics repetidos si el personaje está a mitad de un salto
        if (isStepExecuting) return; 

        currentStepIndex++;

        // Condición de cierre si se acaban los pasos configurados
        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        ExecuteStep(tutorialSteps[currentStepIndex]);
    }

    private void ExecuteStep(TutorialStep step)
    {
        isStepExecuting = true;

        // 1. Actualizar el diálogo/texto de forma instantánea
        if (dialogueTextMesh != null)
        {
            dialogueTextMesh.text = step.dialogueText;
        }

        // 2. Resolver la lógica de movimiento y enfoque en orden
        if (step.moveCharacter)
        {
            // Gelly salta. Al caer, se ejecuta el foco de la UI (Callback)
            gellyController.JumpTo(step.targetCharacterPosition, () => 
            {
                TriggerUIFocus(step);
                isStepExecuting = false; // Liberamos el botón para el siguiente paso
            });
        }
        else
        {
            // Si Gelly no se mueve, el foco de la UI se ejecuta de inmediato
            TriggerUIFocus(step);
            isStepExecuting = false;
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
            focusController.HideFocus(); // Si este paso no usa foco, ocultamos el hoyo
        }
    }

    private void EndTutorial()
    {
        focusController.HideFocus();
        if (dialogueTextMesh != null) dialogueTextMesh.text = "¡Fin del entrenamiento! (⌐■_■)";
        // Aquí puedes desactivar el panel de diálogos o cargar la siguiente escena
    }
}