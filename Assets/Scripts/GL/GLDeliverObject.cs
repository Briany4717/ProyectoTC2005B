using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Maneja la zona de entrega (drop zone) donde se sueltan las órdenes para ser evaluadas.
/// </summary>
public class GLDeliverObject : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// Verifica si el objeto soltado es una orden completada y la procesa en caso afirmativo.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag == null) return;
        
        OrderUI orderDrag = eventData.pointerDrag.GetComponent<OrderUI>();
        if (orderDrag == null)
        {
            Debug.Log("No se encontro la orden arrastrada");
            return;
        }

        GLDragDrop dragData = eventData.pointerDrag.GetComponent<GLDragDrop>();
        if (dragData == null) return;

        if (orderDrag.IsOrderCompleted())
        {
            dragData.MarkAsDropped();
            OrderManager.Instance.DeliverOrder(orderDrag);
        }
        else
        {
            dragData.ReturnToStart();
        }
    }
}