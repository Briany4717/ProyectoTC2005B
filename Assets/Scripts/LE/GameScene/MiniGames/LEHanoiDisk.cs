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
        
        // Detectamos la cámara del Canvas de forma segura
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        uiCamera = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? rootCanvas.worldCamera : Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!hanoiManager.IsMinigameActive || hanoiManager.RepairManager.currentState == LERepairManager.RepairState.Paused) return;

        originalPegParent = transform.parent;

        // UX CRÍTICA: Solo puedes arrastrar el disco si es el que está HASTA ARRIBA de su torre
        if (hanoiManager.GetTopDisk(originalPegParent) != rectTransform)
        {
            eventData.pointerDrag = null; // Cancela el evento de arrastre de raíz
            return;
        }

        hanoiManager.SetDiskVisualState(rectTransform, isSelected: true);
        canvasGroup.blocksRaycasts = false; // Deja pasar el mouse para leer el drop en la torre inferior

        // Lo movemos temporalmente a la capa overlay para que flote por encima de todo el Canvas
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

        // Iteramos sobre las 3 torres para ver si el mouse se soltó dentro de los límites de alguna
        for (int i = 0; i < hanoiManager.towerPegs.Length; i++)
        {
            RectTransform pegRect = hanoiManager.towerPegs[i] as RectTransform;
            if (pegRect != null && RectTransformUtility.RectangleContainsScreenPoint(pegRect, eventData.position, uiCamera))
            {
                targetPeg = hanoiManager.towerPegs[i];
                break;
            }
        }

        // Si se soltó en zona muerta o fuera de una torre, targetPeg será null e irá a las reglas de retorno
        if (targetPeg != null)
        {
            // Devolvemos al padre original un frame para que las matemáticas de jerarquía cuadren
            transform.SetParent(originalPegParent, false);
            hanoiManager.ExecuteMoveRules(originalPegParent, targetPeg);
        }
        else
        {
            // Drop inválido en el vacío: Regresa elásticamente a su torre de origen
            transform.SetParent(originalPegParent, false);
            transform.SetAsLastSibling();
        }
    }
}
