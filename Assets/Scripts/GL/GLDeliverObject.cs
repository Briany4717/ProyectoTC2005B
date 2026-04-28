using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GLDeliverObject : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag == null) return;
        // obtenemos la orden que estamos arrastrando
        OrderUI orderDrag = eventData.pointerDrag.GetComponent<OrderUI>();
        if (orderDrag == null)
        {
            Debug.Log("No se encontro la orden arrastrada");
            return;
        }

        // informacion del drag
        GLDragDrop dragData = eventData.pointerDrag.GetComponent<GLDragDrop>();
        if (dragData == null) return;

        if (orderDrag == null)
        {
            Debug.Log("No se encontro la orden arrastrada");
            return;
        }


        // necesito obtener la orden
        // checar si la orden tiene las estaciones
        if (orderDrag.IsOrderCompleted())
        {
            // si todas las estaicones listas 
            // deliver order
            dragData.MarkAsDropped();
            OrderManager.Instance.DeliverOrder(orderDrag);
        }
        else
        {
            // falta alguna estacion
            // regresar al lugar de la orden
            dragData.ReturnToStart();
        }


    }
}
