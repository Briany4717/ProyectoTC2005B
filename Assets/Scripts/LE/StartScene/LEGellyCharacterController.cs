using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    public enum PivotAlignment 
    { 
        StayOnGround_CenterPivot, 
        StayOnGround_BottomPivot  
    }

    [Header("Horizontal Movement (Parent)")]
    [SerializeField] private AnimationCurve horizontalCurve; 
    [SerializeField] private float duration = 0.6f;

    [Header("Jump Math (Parent)")]
    [SerializeField] private float maxJumpHeight = 2.0f;  
    [SerializeField] private float anticipationDelay = 0.2f; 
    [SerializeField] private float landingAnticipationTime = 0.12f; 

    [Header("Uniform Scale & Absolute Adaptation")]
    [SerializeField] private PivotAlignment alignment = PivotAlignment.StayOnGround_CenterPivot;
    [Tooltip("Altura real en unidades de tu personaje. Esencial para calcular la compensación.")]
    [SerializeField] private float visualHeight = 1.0f; 
    [SerializeField] private AnimationCurve jumpScaleCurve; 
    [SerializeField] private AnimationCurve anticipationScaleCurve;

    [Header("References")]
    public Transform gellyTransformParent; // Mueve su posición GLOBAL absoluta
    public Animator gellyAnimatorChild;   // Su posición local se queda clavada en (0,0,0)

    private float elapsedTime = 0f;
    private bool isJumping = false;
    private bool landTriggered = false; 
    
    // Coordenadas globales puras del suelo
    private Vector3 groundStartPos;
    private Vector3 groundEndPos;
    
    private float startScale;
    private float targetScale;
    
    private System.Action onJumpCompleteCallback;

    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int LandTriggerHash = Animator.StringToHash("Land");

    public void JumpTo(Vector2 targetWorldPos, float nextScale, System.Action onComplete = null)
    {
        if (isJumping) return;

        // El hijo se queda estricto en el origen local para evitar deformaciones
        if (gellyAnimatorChild != null)
        {
            startScale = gellyAnimatorChild.transform.localScale.y;
        }
        
        targetScale = nextScale;
        onJumpCompleteCallback = onComplete;

        // CAMBIO CARDINAL: Capturamos la posición GLOBAL (position) del Padre
        groundStartPos = new Vector3(gellyTransformParent.position.x, CalculateCurrentGroundY(), gellyTransformParent.position.z);
        groundEndPos = new Vector3(targetWorldPos.x, targetWorldPos.y, groundStartPos.z);
        
        elapsedTime = 0f;
        isJumping = true;
        landTriggered = false; 

        if (gellyAnimatorChild != null)
        {
            gellyAnimatorChild.SetTrigger(JumpTriggerHash);
        }
    }

    void Update()
    {
        if (!isJumping) return;

        elapsedTime += Time.deltaTime;

        // FASE 1: Anticipación (Suelo)
        if (elapsedTime < anticipationDelay)
        {
            float antiProgress = elapsedTime / anticipationDelay;
            float currentScale = gellyAnimatorChild.transform.localScale.y;
            
            UpdateTargetTransformations(groundStartPos, 0f, currentScale);
            return;
        }

        // FASE 2: Vuelo
        float jumpTime = elapsedTime - anticipationDelay;
        float t = Mathf.Clamp01(jumpTime / duration);

        float hProgress = horizontalCurve.Evaluate(t);
        Vector3 currentGroundPos = Vector3.LerpUnclamped(groundStartPos, groundEndPos, hProgress);

        float jumpProgress = 4f * t * (1f - t); 
        float currentJumpHeight = jumpProgress * maxJumpHeight;

        float baseScaleAtFrame = Mathf.LerpUnclamped(startScale, targetScale, t);
        float currentScaleAtFrame = baseScaleAtFrame * jumpScaleCurve.Evaluate(t);

        // Actualizamos transformaciones en espacio global
        UpdateTargetTransformations(currentGroundPos, currentJumpHeight, currentScaleAtFrame);

        // Anticipación de Aterrizaje
        float remainingFlightTime = duration - jumpTime;
        if (remainingFlightTime <= landingAnticipationTime && !landTriggered)
        {
            landTriggered = true; 
            if (gellyAnimatorChild != null) gellyAnimatorChild.SetTrigger(LandTriggerHash);
        }

        // FASE 3: Fin del salto
        if (t >= 1f)
        {
            isJumping = false;
            
            UpdateTargetTransformations(groundEndPos, 0f, targetScale);

            if (!landTriggered && gellyAnimatorChild != null)
            {
                gellyAnimatorChild.SetTrigger(LandTriggerHash);
            }

            onJumpCompleteCallback?.Invoke();
        }
    }

    private void UpdateTargetTransformations(Vector3 groundPosition, float jumpHeight, float currentScale)
    {
        if (gellyAnimatorChild == null) return;

        // Escala uniforme al hijo
        gellyAnimatorChild.transform.localScale = new Vector3(currentScale, currentScale, 1f);

        // Compensación absoluta de coordenadas respecto al suelo global
        float pivotOffset = 0f;
        if (alignment == PivotAlignment.StayOnGround_CenterPivot)
        {
            pivotOffset = (visualHeight * 0.5f) * currentScale;
        }

        // CAMBIO CARDINAL: Modificamos la posición global pura del Padre
        gellyTransformParent.position = new Vector3(
            groundPosition.x,
            groundPosition.y + jumpHeight + pivotOffset,
            groundPosition.z
        );
    }

    private float CalculateCurrentGroundY()
    {
        if (alignment == PivotAlignment.StayOnGround_CenterPivot && gellyAnimatorChild != null)
        {
            float currentScale = gellyAnimatorChild.transform.localScale.y;
            return gellyTransformParent.position.y - ((visualHeight * 0.5f) * currentScale);
        }
        return gellyTransformParent.position.y;
    }
}
