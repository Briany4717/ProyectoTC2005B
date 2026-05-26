using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LERepairTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tool Properties")]
    [SerializeField] private int toolId;
    [Tooltip("Descripción personalizada de esta herramienta para el Tooltip dinámico.")]
    [SerializeField, TextArea(2, 3)] private string toolDescription;

    [Header("UI Tooltip Shared Container (⌐■_■)")]
    [SerializeField] private GameObject sharedTooltipPanel; 
    [SerializeField] private TextMeshProUGUI sharedTooltipTextMesh; // <--- TEXTO DINÁMICO ÚNICO
    [SerializeField] private RectTransform applianceHitboxRect; 

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalAnchoredPosition;
    private Camera uiCamera;
    private LERepairManager repairManager;

    void Awake()
    {
        rectTransform = GetComponent<Transform>() as RectTransform;
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        
        repairManager = FindAnyObjectByType<LERepairManager>();
        uiCamera = CanvasCameraDetectorReference();
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (repairManager.currentState == LERepairManager.RepairState.Paused) return;

        if (sharedTooltipPanel != null) sharedTooltipPanel.SetActive(false);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (repairManager.currentState == LERepairManager.RepairState.Paused) return;

        Vector3 mouseWorldPos = uiCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (repairManager.currentState == LERepairManager.RepairState.Paused)
        {
            rectTransform.anchoredPosition = originalAnchoredPosition;
            return;
        }

        if (applianceHitboxRect != null && RectTransformUtility.RectangleContainsScreenPoint(applianceHitboxRect, eventData.position, uiCamera))
        {
            repairManager.ProcessToolDropped(toolId);
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (repairManager.currentState == LERepairManager.RepairState.Paused) return;

        // INYECCIÓN DINÁMICA PREMIUM: Modificamos el contenido antes de prender el contenedor compartido
        if (sharedTooltipPanel != null && sharedTooltipTextMesh != null)
        {
            sharedTooltipTextMesh.text = toolDescription;
            sharedTooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sharedTooltipPanel != null)
        {
            sharedTooltipPanel.SetActive(false);
        }
    }

    private Camera CanvasCameraDetectorReference()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        return (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? rootCanvas.worldCamera : Camera.main;
    }
}
