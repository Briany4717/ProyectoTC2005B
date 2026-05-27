using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class LEAppliance : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ApplianceState { InPool, OnConveyor, BeingDragged, OnWorkbench }
    
    [Header("State")]
    public ApplianceState currentState = ApplianceState.InPool;

    [Header("Visual Randomization  ")]
    [Tooltip("Arrastra aquí todos los sprites de tus 5 electrodomésticos (licuadora, refri, etc.)")]
    [SerializeField] private Sprite[] possibleSprites;

    [Header("Procedural Shake Settings")]
    [SerializeField] private float shakeSpeed = 60f;
    [SerializeField] private float shakeIntensity = 4.0f; 
    [SerializeField] private float moveSpeed = 10f;

    private RectTransform rectTransform;
    private Image applianceImage;
    private Vector3 targetPosition;
    
    // Tracking lineal inmune a la contaminación del sacudido sinusoidal
    private Vector3 smoothPosition; 
    private ApplianceState previousState; 
    
    private Camera mainCamera;
    private LEConveyorManager conveyorManager;

    // SISTEMA DE PAUSA  
    private bool isPaused = false; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        applianceImage = GetComponent<Image>();
        mainCamera = Camera.main;
        conveyorManager = FindAnyObjectByType<LEConveyorManager>();
    }

    public void SetupInConveyor(Vector3 startPos)
    {
        if (applianceImage == null) applianceImage = GetComponent<Image>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (conveyorManager == null) conveyorManager = FindAnyObjectByType<LEConveyorManager>();

        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            applianceImage.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }

        transform.position = startPos;
        smoothPosition = startPos; 
        targetPosition = startPos;
        currentState = ApplianceState.OnConveyor;
    }

    /// <summary>
    /// Asigna el nuevo objetivo en la cola calculado por el ConveyorManager.
    /// </summary>
    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    /// <summary>
    /// ¡REINTEGRADA!: Controla el estado de pausa individual para detener actualizaciones e interacciones.
    /// </summary>
    public void SetPauseState(bool paused)
    {
        isPaused = paused;

        // DETALLE DE UX EXTREMA: Si pausan el juego en pleno drag, forzamos el drop de seguridad
        if (isPaused && currentState == ApplianceState.BeingDragged)
        {
            currentState = previousState;
            if (currentState == ApplianceState.OnWorkbench)
            {
                LEWorkbench workbench = FindAnyObjectByType<LEWorkbench>();
                if (workbench != null) targetPosition = workbench.SlotCenter.position;
            }
        }
    }

    void Update()
    {
        // Si está pausado o se está arrastrando, detenemos cualquier interpolación o sacudido lineal
        if (isPaused || currentState == ApplianceState.BeingDragged) return;

        // La interpolación elástica se calcula sobre la posición limpia
        smoothPosition = Vector3.Lerp(smoothPosition, targetPosition, Time.deltaTime * moveSpeed);

        float distanceToTarget = Vector3.Distance(smoothPosition, targetPosition);
        Vector3 shakeOffset = Vector3.zero;

        // El sacudido SOLO se activa si se mueve en la cinta y no ha llegado a su fila
        if (currentState == ApplianceState.OnConveyor && distanceToTarget > 0.05f)
        {
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            shakeOffset = new Vector3(shakeX, 0f, 0f);
        }

        // Aplicamos la posición final real combinando ambos vectores
        transform.position = smoothPosition + shakeOffset;
    }

    // ====================================================================
    // CANDADOS DE INTERACCIÓN: Si está pausado, bloqueamos los eventos del Canvas
    // ====================================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPaused) return; // Candado de pausa

        if (currentState == ApplianceState.OnConveyor || currentState == ApplianceState.OnWorkbench)
        {
            previousState = currentState;
            currentState = ApplianceState.BeingDragged;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPaused || currentState != ApplianceState.BeingDragged) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        
        transform.position = mouseWorldPos;
        smoothPosition = mouseWorldPos; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPaused || currentState != ApplianceState.BeingDragged) return;

        LEWorkbench workbench = FindAnyObjectByType<LEWorkbench>();
        LEDiscardBox discardBox = FindAnyObjectByType<LEDiscardBox>();

        // CASO 1: Descarte válido desde Cinta o Mesa
        if (discardBox != null && discardBox.TryDiscardAppliance(eventData.position, eventData.pressEventCamera))
        {
            if (previousState == ApplianceState.OnConveyor)
            {
                conveyorManager.RemoveFromConveyor(this);
            }
            else if (previousState == ApplianceState.OnWorkbench && workbench != null)
            {
                workbench.ClearWorkbench(); 
            }

            conveyorManager.RegisterDiscard(this);
            return;
        }

        // CASO 2: Colocación válida en Mesa de Trabajo
        if (workbench != null && workbench.TryPlaceAppliance(this))
        {
            if (previousState == ApplianceState.OnConveyor)
            {
                conveyorManager.RemoveFromConveyor(this);
            }

            currentState = ApplianceState.OnWorkbench;
            targetPosition = workbench.SlotCenter.position;
            return;
        }

        // CASO 3: Cancelación/Drop Inválido (Retorno elástico)
        currentState = previousState;
        if (currentState == ApplianceState.OnWorkbench && workbench != null)
        {
            targetPosition = workbench.SlotCenter.position;
        }
    }
}
