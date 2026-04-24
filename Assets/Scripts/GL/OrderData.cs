using System.Collections.Generic;
using UnityEngine;

// datos que contendrá la ordens
[System.Serializable]
public class OrderData
{
    public int rewardCoins;
    public Sprite foodIcon;
    public List<StationData> requiredStations;
    public List<bool> stationsCompleted;
}

