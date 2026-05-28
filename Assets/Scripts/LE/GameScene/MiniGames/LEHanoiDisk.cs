using UnityEngine;
using UnityEngine.EventSystems;

public class LEHanoiDisk : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private LEHanoiMinigame hanoiManager;
    private RectTransform rectTransform;
    private Transform originalPegParent;
    private CanvasGroup canvasGroup;
    private Camera uiCamera;

    public void SetupDisk(LEHanoiMinigame manager, RectTransform rt)
    {
        hanoiManager = manager;
        rectTransform = rt;
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        uiCamera = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? rootCanvas.worldCamera : Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!hanoiManager.IsMinigameActive || hanoiManager.RepairManager.currentState == LERepairManager.RepairState.Paused) return;

        originalPegParent = transform.parent;

        if (hanoiManager.GetTopDisk(originalPegParent) != rectTransform)
        {
            eventData.pointerDrag = null;
            return;
        }

        hanoiManager.SetDiskVisualState(rectTransform, isSelected: true);
        canvasGroup.blocksRaycasts = false;

        if (hanoiManager.DragOverlayLayer != null)
        {
            transform.SetParent(hanoiManager.DragOverlayLayer, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = uiCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        hanoiManager.SetDiskVisualState(rectTransform, isSelected: false);

        Transform targetPeg = null;

        for (int i = 0; i < hanoiManager.towerPegs.Length; i++)
        {
            RectTransform pegRect = hanoiManager.towerPegs[i] as RectTransform;
            if (pegRect != null && RectTransformUtility.RectangleContainsScreenPoint(pegRect, eventData.position, uiCamera))
            {
                targetPeg = hanoiManager.towerPegs[i];
                break;
            }
        }

        if (targetPeg != null)
        {
            transform.SetParent(originalPegParent, false);
            hanoiManager.ExecuteMoveRules(originalPegParent, targetPeg);
        }
        else
        {
            transform.SetParent(originalPegParent, false);
            transform.SetAsLastSibling();
        }
    }
}
