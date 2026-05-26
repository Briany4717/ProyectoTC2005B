using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LEWorkbench : MonoBehaviour
{
    [Header("Workbench Layout")]
    [SerializeField] private Transform slotCenter; 
    [SerializeField] private Button repairButton;   

    private LEAppliance currentApplianceOnTable = null;

    public Transform SlotCenter => slotCenter;

    public void OnClickRepairButton()
    {
        if (currentApplianceOnTable == null) return;

        LEConveyorManager conveyorManager = FindAnyObjectByType<LEConveyorManager>();

        // 1. EXTRAEMOS Y EMBALAMOS LOS DATOS DE LA ESCENA ACTUAL (⌐■_■)
        LEGameSessionData.Instance.remainingTime = conveyorManager.GetRemainingTime(); // (Crea un getter público en tu mánager que regrese gameTimer)
        LEGameSessionData.Instance.currentApplianceSprite = currentApplianceOnTable.GetComponent<Image>().sprite;
        LEGameSessionData.Instance.repairedCount = conveyorManager.GetRepairedCount();
        LEGameSessionData.Instance.discardedCount = conveyorManager.GetDiscardedCount();

        // 2. SALTO MECÁNICO: Viajamos limpios a la escena de reparación
        SceneManager.LoadScene("LERepairScene");
    }

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
