using UnityEngine;
using UnityEngine.UI;

public class LEWorkbench : MonoBehaviour
{
    [Header("Workbench Layout")]
    [SerializeField] private Transform slotCenter; 
    [SerializeField] private Button repairButton;   

    private LEAppliance currentApplianceOnTable = null;

    public Transform SlotCenter => slotCenter;

    void Start()
    {
        UpdateButtonState();
    }

    public bool TryPlaceAppliance(LEAppliance appliance)
    {
        if (currentApplianceOnTable != null) return false;

        // Validar cercanía al slot de la mesa mediante un radio de tolerancia
        float distance = Vector2.Distance(appliance.transform.position, slotCenter.position);
        if (distance > 2.0f) return false; 

        currentApplianceOnTable = appliance;
        UpdateButtonState();
        return true;
    }

    /// <summary>
    /// LLÁMAME al regresar de la escena de minijuego tras una reparación exitosa (⌐■_■)
    /// </summary>
    public void CompleteActiveRepair()
    {
        if (currentApplianceOnTable == null) return;

        LEConveyorManager conveyorManager = FindAnyObjectByType<LEConveyorManager>();
        if (conveyorManager != null)
        {
            conveyorManager.RegisterRepair(currentApplianceOnTable);
        }

        currentApplianceOnTable = null;
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (repairButton != null)
        {
            repairButton.interactable = (currentApplianceOnTable != null);
        }
    }
}
