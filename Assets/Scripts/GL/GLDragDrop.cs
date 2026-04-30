using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GLDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // elementos para regresar la orden si no se completa
    private Vector2 initialAnchoredPosition;
    private Transform initialParent;


    private bool wasDroppedOnValidZone = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();




    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("OnBeginDrag!!");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;


        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialParent = transform.parent;

        // OrderUI orderUI = GetComponent<OrderUI>();
        // OrderManager.Instance.SelectOrder(orderUI);


    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag!!");
        // esto mueve el mouse la posicion anterior del mouse
        rectTransform.anchoredPosition += eventData.delta;

    }
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
    // se llama cuando apretamos el boton encima del objeto
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Order pressed!!");
        OrderUI orderPressed = GetComponent<OrderUI>();
        OrderManager.Instance.SelectOrder(orderPressed);
    }
    public void ReturnToStart()
    {
        transform.SetParent(initialParent, false);
        rectTransform.anchoredPosition = initialAnchoredPosition;
    }

    public void MarkAsDropped()
    {
        wasDroppedOnValidZone = true;
    }


}
