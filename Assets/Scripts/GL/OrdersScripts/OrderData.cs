using System.Collections.Generic;
using UnityEngine;


/// Estructura de datos que define los elementos requeridos y el progreso de una orden.

[System.Serializable]
public class OrderData
{
    public int rewardCoins;
    public Sprite foodIcon;
    public List<StationData> requiredStations;
    public List<bool> stationsCompleted;
}