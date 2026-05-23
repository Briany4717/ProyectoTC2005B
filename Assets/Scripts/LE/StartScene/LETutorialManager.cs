using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private LEGellyCharacterController gellyShared;
    [SerializeField] private LETutorialFocusController focusShared;

    [Header("UI Elements to Focus")]
    [SerializeField] private RectTransform step1Button;
    [SerializeField] private RectTransform step2Menu;
    [SerializeField] private RectTransform step3Inventory;

    private int currentStep = 0;

    void Start()
    {
        // Inicializamos la posición inicial de Gelly en una tupla
        gellyShared.actualPosition = (gellyShared.transform.position.x, gellyShared.transform.position.y);
        
        // Arrancamos el tutorial automáticamente
        NextStep();
    }

    public void NextStep()
    {
        currentStep++;

        switch (currentStep)
        {
            case 1:
                // PASO 1: Gelly viaja a una posición y al llegar ilumina el primer botón
                gellyShared.MoveTo((-2f, 1f), () => {
                    focusShared.FocusOnElement(step1Button, 0.04f);
                });
                break;

            case 2:
                // PASO 2: Gelly se mueve a otro lado y el foco de la UI viaja fluidamente al menú
                gellyShared.MoveTo((3f, -1.5f), () => {
                    focusShared.FocusOnElement(step2Menu, 0.08f);
                });
                break;

            case 3:
                // PASO 3: Foco en el inventario
                gellyShared.MoveTo((0f, 0f), () => {
                    focusShared.FocusOnElement(step3Inventory, 0.02f);
                });
                break;

            case 4:
                // FIN DEL TUTORIAL: Gelly celebra y quitamos el fondo oscuro
                focusShared.HideFocus();
                Debug.Log("¡Tutorial Completado con éxito! (⌐■_■)");
                break;
        }
    }

    // Un método rápido para que puedas avanzar de paso presionando la barra espaciadora al probar
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextStep();
        }
    }
}
