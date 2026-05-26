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
        // Si el objeto que se soltó es el mismo que ya estaba en la mesa, es totalmente válido (re-drag cancelado)
        if (currentApplianceOnTable == appliance) return true;

        // Si la mesa ya tiene otro objeto diferente bloqueado, deniega el acceso
        if (currentApplianceOnTable != null) return false;

        float distance = Vector2.Distance(appliance.transform.position, slotCenter.position);
        if (distance > 2.0f) return false; 

        currentApplianceOnTable = appliance;
        UpdateButtonState();
        return true;
    }

    /// <summary>
    /// Remueve el registro del electrodoméstico actual liberando la mesa por completo (⌐■_■)
    /// </summary>
    public void ClearWorkbench()
    {
        currentApplianceOnTable = null;
        UpdateButtonState();
    }

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
