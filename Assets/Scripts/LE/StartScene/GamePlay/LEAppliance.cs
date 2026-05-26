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
    [SerializeField] private float shakeIntensity = 4.0f; 
    [SerializeField] private float moveSpeed = 10f;

    private RectTransform rectTransform;
    private Image applianceImage;
    private Vector3 targetPosition;
    
    // GUARDIÁN DE RENDIMIENTO: Almacena la posición lineal pura sin la contaminación del shake
    private Vector3 smoothPosition; 
    private ApplianceState previousState; 
    
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
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            applianceImage.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }

        transform.position = startPos;
        smoothPosition = startPos; // Sincronizamos el tracking base
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

        // El cálculo elástico ocurre sobre la variable limpia e inmune al sacudido
        smoothPosition = Vector3.Lerp(smoothPosition, targetPosition, Time.deltaTime * moveSpeed);

        // REGLA SOLUCIONADA: Evaluamos la distancia real usando el vector limpio para evitar falsos positivos
        float distanceToTarget = Vector3.Distance(smoothPosition, targetPosition);

        Vector3 shakeOffset = Vector3.zero;

        // El sacudido SOLO se activa si está en la cinta Y la distancia lógica es mayor al umbral de parada
        if (currentState == ApplianceState.OnConveyor && distanceToTarget > 0.05f)
        {
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            shakeOffset = new Vector3(shakeX, 0f, 0f);
        }

        // Combinamos la traslación pura con el efecto visual decorativo
        transform.position = smoothPosition + shakeOffset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // DESBLOQUEO: Ahora permitimos el drag tanto si viene de la cinta como si ya estaba en la mesa
        if (currentState == ApplianceState.OnConveyor || currentState == ApplianceState.OnWorkbench)
        {
            previousState = currentState;
            currentState = ApplianceState.BeingDragged;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState != ApplianceState.BeingDragged) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = 0f;
        
        transform.position = mouseWorldPos;
        smoothPosition = mouseWorldPos; // Mantenemos el tracking sincronizado con el puntero
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentState != ApplianceState.BeingDragged) return;

        LEWorkbench workbench = FindAnyObjectByType<LEWorkbench>();
        LEDiscardBox discardBox = FindAnyObjectByType<LEDiscardBox>();

        // CASO 1: Tirar a la Caja de Descartes
        if (discardBox != null && discardBox.TryDiscardAppliance(eventData.position, eventData.pressEventCamera))
        {
            if (previousState == ApplianceState.OnConveyor)
            {
                conveyorManager.RemoveFromConveyor(this);
            }
            else if (previousState == ApplianceState.OnWorkbench && workbench != null)
            {
                workbench.ClearWorkbench(); // Limpia el slot de la mesa para poder meter otro
            }

            conveyorManager.RegisterDiscard(this);
            return;
        }

        // CASO 2: Colocar en la Mesa de Trabajo
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

        // CASO 3: Drop Inválido (Regresa de forma elástica a donde pertenecía originalmente)
        currentState = previousState;
        if (currentState == ApplianceState.OnWorkbench && workbench != null)
        {
            targetPosition = workbench.SlotCenter.position;
        }
    }
}
