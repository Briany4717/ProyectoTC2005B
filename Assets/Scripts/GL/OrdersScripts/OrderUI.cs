using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using Unity.VisualScripting;

/// <summary>
/// Gestiona la visualización en la interfaz de usuario para una orden específica.
/// </summary>
public class OrderUI : MonoBehaviour
{
    public OrderData CurrentOrderData { get; private set; }
    public Image imageBackground;
    public TextMeshProUGUI coinsText;
    public Image foodIconImage;
    public Transform stationsContainer;
    public GameObject stationStepPrefab;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public int rewardCoins = 0;

    private List<StationStepUI> stationSteps = new List<StationStepUI>();

    /// <summary>
    /// Configura y muestra los datos iniciales de una nueva orden.
    /// </summary>
    public void InitializeOrder(OrderData order)
    {
        CurrentOrderData = order;
        coinsText.text = order.rewardCoins.ToString();
        foodIconImage.sprite = order.foodIcon;
        rewardCoins = order.rewardCoins;

        foreach (Transform child in stationsContainer)
        {
            Destroy(child.gameObject);
        }
        stationSteps.Clear();

        foreach (var station in order.requiredStations)
        {
            GameObject stepObj = Instantiate(stationStepPrefab, stationsContainer);
            StationStepUI stepUI = stepObj.GetComponent<StationStepUI>();
            stepUI.Setup(station);
            stationSteps.Add(stepUI);
        }
    }

    /// <summary>
    /// Intenta marcar como completada una estación si pertenece a esta orden.
    /// </summary>
    public bool TryCompleteStation(StationData finishedStation)
    {
        for (int i = 0; i < CurrentOrderData.requiredStations.Count; i++)
        {
            if (CurrentOrderData.requiredStations[i] == finishedStation && !CurrentOrderData.stationsCompleted[i])
            {
                CurrentOrderData.stationsCompleted[i] = true;
                UpdateStationProgress(i);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Actualiza visualmente el progreso de una estación específica en la UI.
    /// </summary>
    public void UpdateStationProgress(int stationIndex)
    {
        if (stationIndex >= 0 && stationIndex < stationSteps.Count)
        {
            stationSteps[stationIndex].MarkAsCompleted();
        }
    }

    /// <summary>
    /// Cambia el color de fondo para indicar si la orden está seleccionada o no.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        imageBackground.color = isSelected ? selectedColor : normalColor;
    }

    /// <summary>
    /// Comprueba si se han completado todas las estaciones de esta orden.
    /// </summary>
    public bool IsOrderCompleted()
    {
        for (int i = 0; i < stationSteps.Count; i++)
        {
            if (!CurrentOrderData.stationsCompleted[i])
            {
                return false;
            }
        }
        return true;
    }
}