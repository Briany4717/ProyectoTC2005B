using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la interfaz visual de una estación individual dentro de una orden.
/// </summary>
public class StationStepUI : MonoBehaviour
{
    public Image stationIconImage;
    public Image statusDotImage;
    public Color incompleteColor = Color.red;
    public Color completeColor = Color.green;

    /// <summary>
    /// Configura el ícono de la estación y su estado inicial como incompleto.
    /// </summary>
    public void Setup(StationData data)
    {
        stationIconImage.sprite = data.stationIcon;
        statusDotImage.color = incompleteColor;
    }

    /// <summary>
    /// Cambia el color del indicador visual para marcar la estación como completada.
    /// </summary>
    public void MarkAsCompleted()
    {
        statusDotImage.color = completeColor;
    }
}