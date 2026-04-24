using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Pool de Datos")]
    public Sprite[] possibleFoodIcons;
    public StationData[] allPossibleStations;

    [Header("Pool de Datos")]
    public GameObject orderPrefab;
    public Transform ordersPanel;

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
    }
}
