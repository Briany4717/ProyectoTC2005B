using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LETutorialFocusController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Camera uiCamera;

    private Material dynamicFocusMaterial; 
    private Image focusImage;
    private Vector4 currentShaderRect; 
    private Vector4 targetShaderRect;
    private Vector4 startShaderRect;
    
    private float timer = 0f;
    private bool isTransitioning = false;

    private static readonly int CutoutRectHash = Shader.PropertyToID("_CutoutRect");
    private static readonly int RadiusHash = Shader.PropertyToID("_Radius");

    void Awake()
    {
        focusImage = GetComponent<Image>();
        dynamicFocusMaterial = focusImage.material; 
    }

    public void FocusOnElement(RectTransform targetUI, float cornerRadius = 0.05f)
    {
        if (targetUI == null) return;

        targetShaderRect = CalculateNormalizedRect(targetUI);
        dynamicFocusMaterial.SetFloat(RadiusHash, cornerRadius);

        if (currentShaderRect == Vector4.zero)
        {
            // Inicializa el hoyo en el lugar correcto pero con tamaño cero para un escalado elástico fluido
            currentShaderRect = new Vector4(targetShaderRect.x, targetShaderRect.y, 0f, 0f);
        }

        startShaderRect = currentShaderRect;
        timer = 0f;
        isTransitioning = true;
    }

    public void HideFocus()
    {
        targetShaderRect = Vector4.zero;
        startShaderRect = currentShaderRect;
        timer = 0f;
        isTransitioning = true;
    }

    private Vector4 CalculateNormalizedRect(RectTransform rectTransform)
    {
        // 1. Conseguir las 4 esquinas del objeto UI en espacio del mundo
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 2. Proyectar esquinas a píxeles reales de la pantalla (Indestructible ante cambios de pivote)
        Vector2 screenBottomLeft = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCorners[0]);
        Vector2 screenTopRight = RectTransformUtility.WorldToScreenPoint(uiCamera, worldCorners[2]);

        // 3. Calcular centro y tamaño exacto en píxeles de pantalla
        Vector2 screenCenter = (screenBottomLeft + screenTopRight) * 0.5f;
        float screenWidthPixels = screenTopRight.x - screenBottomLeft.x;
        float screenHeightPixels = screenTopRight.y - screenBottomLeft.y;

        // 4. Normalización estricta (0.0 a 1.0) inmune a cambios de resolución
        return new Vector4(
            screenCenter.x / Screen.width,
            screenCenter.y / Screen.height,
            screenWidthPixels / Screen.width,
            screenHeightPixels / Screen.height
        );
    }

    void Update()
    {
        if (!isTransitioning) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float curveValue = transitionCurve.Evaluate(t);

        currentShaderRect = Vector4.LerpUnclamped(startShaderRect, targetShaderRect, curveValue);
        dynamicFocusMaterial.SetVector(CutoutRectHash, currentShaderRect);

        if (t >= 1f) isTransitioning = false;
    }

    private void OnDisable()
    {
        currentShaderRect = Vector4.zero;
        if (dynamicFocusMaterial != null) dynamicFocusMaterial.SetVector(CutoutRectHash, Vector4.zero);
    }
}
