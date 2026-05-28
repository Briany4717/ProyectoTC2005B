using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;


/// Gestiona la creación, selección y entrega de las órdenes del juego.

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    
    /// Establece el patrón Singleton para la clase.
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    
    /// Inicia la generación automática de órdenes.
    
    private void Start()
    {
        startOrderGenerator();
    }

    private int selectedOrderIndex = -1;

    [Header("Pool de Datos")]
    public Sprite[] possibleFoodIcons;
    public StationData[] allPossibleStations;

    [Header("Pool de Datos")]
    public GameObject orderPrefab;
    public Transform ordersPanel;

    public ParticleSystem particle1;
    public ParticleSystem particle2;

    private OrderUI currentSelectedOrder;
    private List<OrderUI> activeOrders = new List<OrderUI>();

    
    /// Verifica la entrada del teclado para seleccionar las órdenes activas.
    
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectOrder(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectOrder(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectOrder(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectOrder(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectOrder(4);
    }

    
    /// Marca visualmente una orden específica como seleccionada.
    
    public void SelectOrder(OrderUI newOrder)
    {
        if (currentSelectedOrder != null)
        {
            currentSelectedOrder.SetSelected(false);
        }

        currentSelectedOrder = newOrder;

        if (currentSelectedOrder != null)
        {
            currentSelectedOrder.SetSelected(true);
            selectedOrderIndex = activeOrders.IndexOf(currentSelectedOrder);
        }
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.PaperSound);
    }

    
    /// Selecciona una orden activa en base a su índice.
    
    private void SelectOrder(int index)
    {
        if (index >= 0 && index < activeOrders.Count)
        {
            SelectOrder(activeOrders[index]);
        }
    }

    
    /// Verifica si una estación completada corresponde a la orden seleccionada.
    
    public void OnPlayerCompletedStation(StationData stationCompleted)
    {
        if (selectedOrderIndex >= 0 && selectedOrderIndex < activeOrders.Count)
        {
            OrderUI targetOrder = activeOrders[selectedOrderIndex];
            if (targetOrder.TryCompleteStation(stationCompleted))
            {
                Debug.Log("La estacion se completo correctamente");
                particle1.Play();
                particle2.Play();
            }
            else
            {
                Debug.Log("No existe la estacion en la orden");
            }
        }
        else
        {
            Debug.Log("No hay ninguna estacion seleccionada");
        }
    }

    
    /// Entrega una orden completada, otorga puntos y la elimina de la pantalla.
    
    public void DeliverOrder(OrderUI targetOrder)
    {
        Debug.Log("Se completó la orden!!!");
        GLScoreController.Instance.AddOrderCoins(targetOrder.rewardCoins);

        activeOrders.RemoveAt(selectedOrderIndex);
        Destroy(targetOrder.gameObject);
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.JeopardyCorrect);

        if (activeOrders.Count > 0)
        {
            selectedOrderIndex = 0;
            SelectOrder(selectedOrderIndex);
        }
        else
        {
            selectedOrderIndex = -1;
        }
    }

    
    /// Genera una nueva orden aleatoria con íconos y estaciones requeridas.
    
    public void GenerateRandomOrder()
    {
        OrderData newOrder = new OrderData();
        newOrder.rewardCoins = Random.Range(10, 51);
        newOrder.foodIcon = possibleFoodIcons[Random.Range(0, possibleFoodIcons.Length)];

        int numberOfStations = Random.Range(1, 4);
        newOrder.requiredStations = new List<StationData>();
        newOrder.stationsCompleted = new List<bool>();

        for (int i = 0; i < numberOfStations; i++)
        {
            StationData randomStation = allPossibleStations[Random.Range(0, allPossibleStations.Length)];
            newOrder.requiredStations.Add(randomStation);
            newOrder.stationsCompleted.Add(false);
        }

        GameObject orderObj = Instantiate(orderPrefab, ordersPanel);
        OrderUI uiController = orderObj.GetComponent<OrderUI>();
        uiController.InitializeOrder(newOrder);
        activeOrders.Add(uiController);

        if (activeOrders.Count == 1)
        {
            SelectOrder(0);
        }
    }

    
    /// Comienza la corrutina de generación de órdenes por tiempo.
    
    public void startOrderGenerator()
    {
        GenerateRandomOrder();
        StartCoroutine(TimeOrderGenerator());
    }

    
    /// Genera nuevas órdenes aleatorias periódicamente si no se ha alcanzado el límite.
    
    IEnumerator TimeOrderGenerator()
    {
        if (activeOrders.Count < 5)
        {
            int randomTimeToWait = Random.Range(1, 5);
            Debug.Log("Orden Creada!!!!");
            yield return new WaitForSeconds(randomTimeToWait);
            GenerateRandomOrder();
        }

        yield return new WaitForSeconds(2);
        StartCoroutine(TimeOrderGenerator());
    }
}