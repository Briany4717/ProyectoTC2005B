using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    public enum PivotAlignment { StayOnGround_CenterPivot, StayOnGround_BottomPivot }
    [SerializeField] private AnimationCurve horizontalCurve; 
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float maxJumpHeight = 2.0f;  
    [SerializeField] private float anticipationDelay = 0.2f; 
    [SerializeField] private float landingAnticipationTime = 0.12f; 
    [SerializeField] private PivotAlignment alignment = PivotAlignment.StayOnGround_CenterPivot;
    [SerializeField] private float visualHeight = 1.0f; 
    [SerializeField] private AnimationCurve jumpScaleCurve; 
    [SerializeField] private AnimationCurve anticipationScaleCurve;
    public Transform gellyTransformParent; 
    public Animator gellyAnimatorChild;   

    private float elapsedTime = 0f;
    private bool isJumping = false;
    private bool isMovingLinear = false; 
    private bool landTriggered = false; 
    private bool faceRight = false; 
    private Vector3 groundStartPos;
    private Vector3 groundEndPos;
    private float startScale;
    private float targetScale;
    private float currentMovementDuration;
    private AnimationCurve currentMovementCurve;
    private System.Action onMovementCompleteCallback;

    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int JumpVerticalTriggerHash = Animator.StringToHash("JumpVertical"); 
    private static readonly int LandTriggerHash = Animator.StringToHash("Land");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving"); 

    // CONTROLES DE PAUSA QUIRÚRGICOS  
    private bool isPaused = false;
    private float cachedAnimatorSpeed = 1f;

    public void PauseCharacter()
    {
        if (isPaused) return;
        isPaused = true;

        if (gellyAnimatorChild != null)
        {
            cachedAnimatorSpeed = gellyAnimatorChild.speed;
            gellyAnimatorChild.speed = 0f;
        }
    }

    public void ResumeCharacter()
    {
        if (!isPaused) return;
        isPaused = false;

        if (gellyAnimatorChild != null)
        {
            gellyAnimatorChild.speed = cachedAnimatorSpeed;
        }
    }

    void Update()
    {
        if (isPaused) return; 

        if (isJumping) HandleParabolicJump();
        else if (isMovingLinear) HandleLinearMovement();
    }

    public void JumpTo(Vector2 targetWorldPos, float nextScale, System.Action onComplete = null)
    {
        if (isJumping || isMovingLinear) return;
        SetupBaseMovement(targetWorldPos, nextScale, onComplete);
        currentMovementDuration = duration;
        currentMovementCurve = horizontalCurve;
        isJumping = true;
        if (gellyAnimatorChild != null) gellyAnimatorChild.SetTrigger(JumpTriggerHash);
    }

    public void JumpVerticalTo(Vector2 targetWorldPos, float nextScale, System.Action onComplete = null)
    {
        if (isJumping || isMovingLinear) return;
        SetupBaseMovement(targetWorldPos, nextScale, onComplete);
        currentMovementDuration = duration;
        currentMovementCurve = horizontalCurve;
        isJumping = true;
        if (gellyAnimatorChild != null) gellyAnimatorChild.SetTrigger(JumpVerticalTriggerHash);
    }

    public void MoveLinearTo(Vector2 targetWorldPos, float nextScale, float customDuration, AnimationCurve customCurve, System.Action onComplete = null)
    {
        if (isJumping || isMovingLinear) return;
        SetupBaseMovement(targetWorldPos, nextScale, onComplete);
        currentMovementDuration = customDuration;
        currentMovementCurve = customCurve;
        isMovingLinear = true;
        if (gellyAnimatorChild != null) gellyAnimatorChild.SetBool(IsMovingHash, true);
    }

    private void SetupBaseMovement(Vector2 targetWorldPos, float nextScale, System.Action onComplete)
    {
        if (gellyAnimatorChild != null) startScale = gellyAnimatorChild.transform.localScale.y;
        targetScale = nextScale;
        onMovementCompleteCallback = onComplete;
        groundStartPos = new Vector3(gellyTransformParent.position.x, CalculateCurrentGroundY(), gellyTransformParent.position.z);
        groundEndPos = new Vector3(targetWorldPos.x, targetWorldPos.y, groundStartPos.z);
        if (Mathf.Abs(targetWorldPos.x - gellyTransformParent.position.x) > 0.01f) faceRight = targetWorldPos.x > gellyTransformParent.position.x;
        elapsedTime = 0f;
        landTriggered = false;
    }

    private void HandleParabolicJump()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < anticipationDelay)
        {
            float antiProgress = elapsedTime / anticipationDelay;
            float currentScale = gellyAnimatorChild.transform.localScale.y;
            UpdateTargetTransformations(groundStartPos, 0f, currentScale);
            return;
        }
        float jumpTime = elapsedTime - anticipationDelay;
        float t = Mathf.Clamp01(jumpTime / currentMovementDuration);
        float hProgress = currentMovementCurve.Evaluate(t);
        Vector3 currentGroundPos = Vector3.LerpUnclamped(groundStartPos, groundEndPos, hProgress);
        float jumpProgress = 4f * t * (1f - t); 
        float currentJumpHeight = jumpProgress * maxJumpHeight;
        float baseScaleAtFrame = Mathf.LerpUnclamped(startScale, targetScale, t);
        float currentScaleAtFrame = baseScaleAtFrame * jumpScaleCurve.Evaluate(t);
        UpdateTargetTransformations(currentGroundPos, currentJumpHeight, currentScaleAtFrame);
        float remainingFlightTime = currentMovementDuration - jumpTime;
        if (remainingFlightTime <= landingAnticipationTime && !landTriggered)
        {
            landTriggered = true; 
            if (gellyAnimatorChild != null) gellyAnimatorChild.SetTrigger(LandTriggerHash);
        }
        if (t >= 1f)
        {
            isJumping = false;
            faceRight = false; 
            UpdateTargetTransformations(groundEndPos, 0f, targetScale);
            if (!landTriggered && gellyAnimatorChild != null) gellyAnimatorChild.SetTrigger(LandTriggerHash);
            onMovementCompleteCallback?.Invoke();
        }
    }

    private void HandleLinearMovement()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / currentMovementDuration);
        float progress = currentMovementCurve.Evaluate(t);
        Vector3 currentGroundPos = Vector3.LerpUnclamped(groundStartPos, groundEndPos, progress);
        float currentScaleAtFrame = Mathf.LerpUnclamped(startScale, targetScale, progress);
        UpdateTargetTransformations(currentGroundPos, 0f, currentScaleAtFrame);
        if (t >= 1f)
        {
            isMovingLinear = false;
            faceRight = false; 
            UpdateTargetTransformations(groundEndPos, 0f, targetScale);
            if (gellyAnimatorChild != null) gellyAnimatorChild.SetBool(IsMovingHash, false);
            onMovementCompleteCallback?.Invoke();
        }
    }

    private void UpdateTargetTransformations(Vector3 groundPosition, float jumpHeight, float currentScale)
    {
        if (gellyAnimatorChild == null) return;
        float finalScaleX = faceRight ? -currentScale : currentScale;
        gellyAnimatorChild.transform.localScale = new Vector3(finalScaleX, currentScale, 1f);
        float pivotOffset = 0f;
        if (alignment == PivotAlignment.StayOnGround_CenterPivot) pivotOffset = (visualHeight * 0.5f) * currentScale;
        gellyTransformParent.position = new Vector3(groundPosition.x, groundPosition.y + jumpHeight + pivotOffset, groundPosition.z);
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
