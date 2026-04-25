using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using Unity.VisualScripting;

public class OrderUI : MonoBehaviour
{
    // necesitamos guardar los valores de la orden para poder hacer modificaciones
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

    public void InitializeOrder(OrderData order)
    {
        // guardamos la nueva orden para que lo trackee OrderUI
        CurrentOrderData = order;
        // actualizamos valores
        coinsText.text = order.rewardCoins.ToString();
        foodIconImage.sprite = order.foodIcon;
        rewardCoins = order.rewardCoins;

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

    // esta funcion busca una estación que hayamos intentado acabar 
    // y verifica si se encuentra en nuestra orden
    public bool TryCompleteStation(StationData finishedStation)
    {
        for (int i = 0; i < CurrentOrderData.requiredStations.Count; i++)
        {
            // Verificamos dos cosas: 
            // 1. ¿Es la estación que buscamos?
            // 2. ¿AÚN NO está completada?
            if (CurrentOrderData.requiredStations[i] == finishedStation && !CurrentOrderData.stationsCompleted[i])
            {
                // ¡Coincidencia! La marcamos como completada en los datos
                CurrentOrderData.stationsCompleted[i] = true;

                // Actualizamos la bolita verde en la UI
                UpdateStationProgress(i);

                // Retornamos true para avisar que esta orden "se comió" la acción
                return true;
            }
        }
        // Si termina el ciclo y no encontró nada útil, retorna falso
        return false;
    }

    // Llama a esto cuando el jugador complete una estación de esta orden
    public void UpdateStationProgress(int stationIndex)
    {
        if (stationIndex >= 0 && stationIndex < stationSteps.Count)
        {
            stationSteps[stationIndex].MarkAsCompleted();
        }
    }

    public void SetSelected(bool isSelected)
    {
        imageBackground.color = isSelected ? selectedColor : normalColor;
    }

    // checar si la orden ya tiene todas las estaciones hechas
    public bool IsOrderCompleted()
    {

        for (int i = 0; i < stationSteps.Count; i++)
        {
            // checar los estados de 
            if (!CurrentOrderData.stationsCompleted[i])
            {
                // si no esta completado paramos y devolvemos falso
                return false;
            }
        }
        return true;
    }

}
