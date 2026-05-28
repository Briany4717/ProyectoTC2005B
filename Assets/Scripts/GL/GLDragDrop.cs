using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


/// Gestiona la interacción de arrastrar y soltar objetos de interfaz (Drag and Drop).

public class GLDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 initialAnchoredPosition;
    private Transform initialParent;
    private bool wasDroppedOnValidZone = false;

    
    /// Inicializa las referencias a los componentes visuales del elemento arrastrable.
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    
    /// Configura el estado inicial al comenzar el arrastre del objeto.
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag!!");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialParent = transform.parent;
    }

    
    /// Actualiza la posición en pantalla durante el arrastre basado en el movimiento del cursor.
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag!!");
        rectTransform.anchoredPosition += eventData.delta;
    }

    
    /// Restablece el estado visual y comprueba si se soltó en una zona válida.
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag!!");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!wasDroppedOnValidZone)
        {
            ReturnToStart();
        }
    }

    
    /// Selecciona la orden interactuada reproduciendo su sonido al hacer clic sobre ella.
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Order pressed!!");
        OrderUI orderPressed = GetComponent<OrderUI>();
        OrderManager.Instance.SelectOrder(orderPressed);
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.PaperSound);
    }

    
    /// Devuelve el objeto a su posición original si el arrastre no fue exitoso.
    
    public void ReturnToStart()
    {
        transform.SetParent(initialParent, false);
        rectTransform.anchoredPosition = initialAnchoredPosition;
    }

    
    /// Marca el objeto como depositado en una zona correcta, evitando que regrese a su origen.
    
    public void MarkAsDropped()
    {
        wasDroppedOnValidZone = true;
    }
}