using UnityEngine;
// clase para ligar la estacion con su icono
[CreateAssetMenu(fileName = "NewStation", menuName = "Overcooked/Station")]
public class StationData : ScriptableObject
{
    public string stationName;
    public Sprite stationIcon;
}