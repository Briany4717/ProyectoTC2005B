using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;

public class OrderUI : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public Image foodIconImage;
    public Transform stationsContainer;
    public GameObject stationStepPrefab;

    private List<StationStepUI> stationSteps = new List<StationStepUI>();

    public void InitializeOrder(OrderData order)
    {
        coinsText.text = order.rewardCoins.ToString();
        foodIconImage.sprite = order.foodIcon;

        // borramos rastro alguno de estaciones anteriores
        foreach (Transform child in stationsContainer)
        {
            Destroy(child.gameObject);
        }
        stationSteps.Clear();

        // generar los iconos correspondientes a la estacion
        foreach (var station in order.requiredStations)
        {
            GameObject stepObj = Instantiate(stationStepPrefab, stationsContainer);
            StationStepUI stepUI = stepObj.GetComponent<StationStepUI>();
            stepUI.Setup(station);
            stationSteps.Add(stepUI);
        }

    }

    // Llama a esto cuando el jugador complete una estación de esta orden
    public void UpdateStationProgress(int stationIndex)
    {
        if (stationIndex >= 0 && stationIndex < stationSteps.Count)
        {
            stationSteps[stationIndex].MarkAsCompleted();
        }
    }

}
