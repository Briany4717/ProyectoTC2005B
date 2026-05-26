using UnityEngine;

public class LEDiscardBox : MonoBehaviour
{
    [Header("UI Detection Area (⌐■_■)")]
    [Tooltip("Arrastra aquí el RectTransform de la imagen de tu caja de descartes.")]
    [SerializeField] private RectTransform discardRect;

    public bool TryDiscardAppliance(Vector2 screenPoint, Camera uiCamera)
    {
        if (discardRect == null) return false;

        // Interroga directamente al Canvas si las coordenadas del mouse colisionan con el cuadro
        return RectTransformUtility.RectangleContainsScreenPoint(discardRect, screenPoint, uiCamera);
    }
}
