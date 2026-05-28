using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estructura de datos que define los elementos requeridos y el progreso de una orden.
/// </summary>
[System.Serializable]
public class OrderData
{
    public int rewardCoins;
    public Sprite foodIcon;
    public List<StationData> requiredStations;
    public List<bool> stationsCompleted;
}