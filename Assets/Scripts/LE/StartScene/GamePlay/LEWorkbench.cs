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

        if (conveyorManager != null)
        {
            // 1. EXTRAEMOS Y EMBALAMOS LOS DATOS DE LA SESIÓN  
            LEGameSessionData.Instance.remainingTime = conveyorManager.GetRemainingTime(); 
            LEGameSessionData.Instance.currentApplianceSprite = currentApplianceOnTable.GetComponent<Image>().sprite;
            LEGameSessionData.Instance.repairedCount = conveyorManager.GetRepairedCount();
            LEGameSessionData.Instance.discardedCount = conveyorManager.GetDiscardedCount();
            
            // ¡NUEVA LÍNEA CLAVE!: Salvamos el conteo histórico exacto de spawns de la cinta
            LEGameSessionData.Instance.totalSpawnedCount = conveyorManager.GetTotalSpawnedCount();
        }

        // 2. SALTO MECÁNICO SEGÚN TU CONFIGURACIÓN DE BUILD SETTINGS ("LEGameScene")
        SceneManager.LoadScene("LEGameScene");
    }

    void Start()
    {
        UpdateButtonState();
    }

    public bool TryPlaceAppliance(LEAppliance appliance)
    {
        if (currentApplianceOnTable == appliance) return true;
        if (currentApplianceOnTable != null) return false;

        float distance = Vector2.Distance(appliance.transform.position, slotCenter.position);
        if (distance > 2.0f) return false; 

        currentApplianceOnTable = appliance;
        UpdateButtonState();
        return true;
    }

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
