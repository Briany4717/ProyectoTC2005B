using UnityEngine;
using UnityEngine.UI;

// Esta sirve para modificar cada estacion con su imagen
// su estado e inicializarlo.
public class StationStepUI : MonoBehaviour
{
    public Image stationIconImage;
    public Image statusDotImage;
    public Color incompleteColor = Color.red;
    public Color completeColor = Color.green;

    public void Setup(StationData data)
    {
        stationIconImage.sprite = data.stationIcon;
        statusDotImage.color = incompleteColor;
    }

    public void MarkAsCompleted()
    {
        statusDotImage.color = completeColor;
    }
}