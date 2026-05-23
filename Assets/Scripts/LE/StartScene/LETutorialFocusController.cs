using UnityEngine;

public class LETutorialFocusController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material focusMaterial; 
    
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private float duration = 0.5f;

    private Vector4 currentShaderRect; 
    private Vector4 targetShaderRect;
    private Vector4 startShaderRect;
    
    private float timer = 0f;
    private bool isTransitioning = false;

    private static readonly int CutoutRectHash = Shader.PropertyToID("_CutoutRect");
    private static readonly int RadiusHash = Shader.PropertyToID("_Radius");

    public void FocusOnElement(RectTransform targetUI, float cornerRadius = 0.05f)
    {
        if (targetUI == null) return;

        targetShaderRect = CalculateNormalizedRect(targetUI);
        focusMaterial.SetFloat(RadiusHash, cornerRadius);

        if (currentShaderRect == Vector4.zero)
        {
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
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);

        return new Vector4(
            screenPos.x / Screen.width,
            screenPos.y / Screen.height,
            size.x / Screen.width,
            size.y / Screen.height
        );
    }

    void Update()
    {
        if (!isTransitioning) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float curveValue = transitionCurve.Evaluate(t);

        currentShaderRect = Vector4.LerpUnclamped(startShaderRect, targetShaderRect, curveValue);
        focusMaterial.SetVector(CutoutRectHash, currentShaderRect);

        if (t >= 1f) isTransitioning = false;
    }

    private void OnDisable()
    {
        currentShaderRect = Vector4.zero;
        if (focusMaterial != null) focusMaterial.SetVector(CutoutRectHash, Vector4.zero);
    }
}
