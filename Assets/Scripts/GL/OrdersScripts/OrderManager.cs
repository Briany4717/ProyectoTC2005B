using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    private void Awake()
    {
        // Asegurarnos de que solo exista un OrderManager en la escena
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        startOrderGenerator();
    }

    // para saber que orden escogio el usuario
    private int selectedOrderIndex = -1;

    [Header("Pool de Datos")]
    public Sprite[] possibleFoodIcons;
    public StationData[] allPossibleStations;

    [Header("Pool de Datos")]
    public GameObject orderPrefab;
    public Transform ordersPanel;
    public ParticleSystem particle1;
    public ParticleSystem particle2;

    // esta lista guarda todas las ordenes activas en el momento.
    private List<OrderUI> activeOrders = new List<OrderUI>();

    private void Update()
    {
        // Escuchar las teclas del 1 al 5 en la fila superior del teclado
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectOrder(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectOrder(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectOrder(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectOrder(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectOrder(4);
    }

    private void SelectOrder(int index)
    {
        // verificamos que esté dentro del rango
        if (index < activeOrders.Count && index >= 0)
        {
            // apagar la orden anterior
            if (selectedOrderIndex < activeOrders.Count && selectedOrderIndex >= 0)
            {
                activeOrders[selectedOrderIndex].SetSelected(false);
            }
            //actualizamos el nuevo index
            activeOrders[index].SetSelected(true);
            selectedOrderIndex = index;
        }


    }

    public void OnPlayerCompletedStation(StationData stationCompleted)
    {
        // obtenemos la orden actual

        if (selectedOrderIndex >= 0 && selectedOrderIndex < activeOrders.Count)
        {
            //verificamos que lo que este seleccionado sea valido
            // sacar del if si no funciona
            OrderUI targetOrder = activeOrders[selectedOrderIndex];
            // verificamos que en la orden exista la estacion
            if (targetOrder.TryCompleteStation(stationCompleted))
            {
                Debug.Log("La estacion se completo correctamente");
                // reproducir particulas
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

    public void DeliverOrder(OrderUI targetOrder)
    {
        // la estacion se completó llamadmos una funcion para recibir puntos
        Debug.Log("Se completó la orden!!!");
        GLScoreController.Instance.AddOrderCoins(targetOrder.rewardCoins);

        // borrar en la list y visualmente
        activeOrders.RemoveAt(selectedOrderIndex);
        Destroy(targetOrder.gameObject);
        // cambiar el pointer para no romper el juego
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

    public void GenerateRandomOrder()
    {
        OrderData newOrder = new OrderData();

        // initialize with random numbers
        newOrder.rewardCoins = Random.Range(10, 51);
        // initialize all possible icons
        newOrder.foodIcon = possibleFoodIcons[Random.Range(0, possibleFoodIcons.Length)];

        // crear las estaciones aleatorias
        int numberOfStations = Random.Range(1, 4);
        newOrder.requiredStations = new List<StationData>();
        newOrder.stationsCompleted = new List<bool>();

        // agregar las estacions a las ordenes
        for (int i = 0; i < numberOfStations; i++)
        {
            StationData randomStation = allPossibleStations[Random.Range(0, allPossibleStations.Length)];
            newOrder.requiredStations.Add(randomStation);
            newOrder.stationsCompleted.Add(false);
        }

        // instanciar la orden para que se vea en unity
        // usamos un prefab ya hecho
        GameObject orderObj = Instantiate(orderPrefab, ordersPanel);

        // llamamos el ui controller que tiene la funcion para organizar toda la orden.
        OrderUI uiController = orderObj.GetComponent<OrderUI>();
        uiController.InitializeOrder(newOrder);
        // agregamos el controlador a la list de ordenes activas
        activeOrders.Add(uiController);

        // si es la primera orden generada la agregamos.
        if (activeOrders.Count == 1)
        {
            SelectOrder(0);
        }

    }

    public void startOrderGenerator()
    {
        GenerateRandomOrder();
        StartCoroutine(TimeOrderGenerator());
    }
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
