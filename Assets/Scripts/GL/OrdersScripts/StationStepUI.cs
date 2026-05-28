using UnityEngine;
using UnityEngine.UI;


/// Gestiona la interfaz visual de una estación individual dentro de una orden.

public class StationStepUI : MonoBehaviour
{
    public Image stationIconImage;
    public Image statusDotImage;
    public Color incompleteColor = Color.red;
    public Color completeColor = Color.green;

    
    /// Configura el ícono de la estación y su estado inicial como incompleto.
    
    public void Setup(StationData data)
    {
        stationIconImage.sprite = data.stationIcon;
        statusDotImage.color = incompleteColor;
    }

    
    /// Cambia el color del indicador visual para marcar la estación como completada.
    
    public void MarkAsCompleted()
    {
        statusDotImage.color = completeColor;
    }
}