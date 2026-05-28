using UnityEngine;

/// <summary>
/// Contiene los datos configurables de una estación, como su nombre y su ícono.
/// </summary>
[CreateAssetMenu(fileName = "NewStation", menuName = "Overcooked/Station")]
public class StationData : ScriptableObject
{
    public string stationName;
    public Sprite stationIcon;
}