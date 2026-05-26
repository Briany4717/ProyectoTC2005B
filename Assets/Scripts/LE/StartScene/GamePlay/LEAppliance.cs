using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class LEAppliance : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ApplianceState { InPool, OnConveyor, BeingDragged, OnWorkbench }
    
    [Header("State")]
    public ApplianceState currentState = ApplianceState.InPool;

    [Header("Visual Randomization (⌐■_■)")]
    [Tooltip("Arrastra aquí todos los sprites de tus 5 electrodomésticos (licuadora, refri, etc.)")]
    [SerializeField] private Sprite[] possibleSprites;

    [Header("Procedural Shake Settings")]
    [SerializeField] private float shakeSpeed = 60f;
    [SerializeField] private float shakeIntensity = 4.0f; // Ahora en pixeles de UI
    [SerializeField] private float moveSpeed = 10f;

    private RectTransform rectTransform;
    private Image applianceImage;
    private Vector3 targetPosition;
    private Camera mainCamera;
    private LEConveyorManager conveyorManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        applianceImage = GetComponent<Image>();
        mainCamera = Camera.main;
        conveyorManager = FindAnyObjectByType<LEConveyorManager>();
    }

    public void SetupInConveyor(Vector3 startPos)
    {
        // TRUCO DE OPTIMIZACIÓN: Cambiamos el sprite de forma aleatoria antes de aparecer
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            applianceImage.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }

        transform.position = startPos;
        targetPosition = startPos;
        currentState = ApplianceState.OnConveyor;
    }

    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    void Update()
    {
        if (currentState == ApplianceState.BeingDragged) return;

        // Desplazamiento elástico nativo de UI
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // El sacudido procedimental solo ocurre si el objeto se está desplazando por la cinta
        if (currentState == ApplianceState.OnConveyor && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            rectTransform.anchoredPosition += new Vector2(shakeX, 0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentState == ApplianceState.OnWorkbench) return; 
        currentState = ApplianceState.BeingDragged;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState != ApplianceState.BeingDragged) return;

        // Arrastre perfecto y suave amarrado al puntero de la UI
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentState != ApplianceState.BeingDragged) return;

        // 1. Validar Drop en Mesa de Trabajo
        LEWorkbench workbench = FindAnyObjectByType<LEWorkbench>();
        if (workbench != null && workbench.TryPlaceAppliance(this))
        {
            currentState = ApplianceState.OnWorkbench;
            targetPosition = workbench.SlotCenter.position;
            conveyorManager.RemoveFromConveyor(this); 
            return;
        }

        // 2. Validar Drop en Caja de Descartes usando matemática de UI (0% Física)
        LEDiscardBox discardBox = FindAnyObjectByType<LEDiscardBox>();
        if (discardBox != null && discardBox.TryDiscardAppliance(eventData.position, eventData.pressEventCamera))
        {
            conveyorManager.RemoveFromConveyor(this); 
            conveyorManager.RegisterDiscard(this);    
            return;
        }

        // Retorno si se soltó en zona inválida
        currentState = ApplianceState.OnConveyor;
    }
}
