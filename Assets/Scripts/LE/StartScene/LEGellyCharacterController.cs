using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private float duration = 0.8f;
    
    [Header("References")]
    public Transform gellyTransformParent; 
    public Animator gellyAnimatorChild;   

    [HideInInspector] public (float x, float y) actualPosition;
    [HideInInspector] public (float x, float y) nextPosition;

    private float timer = 0f;
    private bool isMoving = false;
    private Vector3 startPos;
    private Vector3 endPos;

    private System.Action onMovementCompleteCallback;
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

    public void MoveTo((float x, float y) targetPos, System.Action onComplete = null)
    {
        if (isMoving) return;

        nextPosition = targetPos;
        startPos = gellyTransformParent.position;
        endPos = new Vector3(nextPosition.x, nextPosition.y, startPos.z);
        
        onMovementCompleteCallback = onComplete;
        timer = 0f;
        isMoving = true;

        if (gellyAnimatorChild != null) gellyAnimatorChild.SetBool(IsMovingHash, true);
    }

    void Update()
    {
        if (!isMoving) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float curveValue = movementCurve.Evaluate(t);

        // Movimiento y escala elásticos unificados en el Padre
        gellyTransformParent.position = Vector3.LerpUnclamped(startPos, endPos, curveValue);
        gellyTransformParent.localScale = Vector3.one * curveValue;

        if (t >= 1f)
        {
            isMoving = false;
            actualPosition = nextPosition;
            
            if (gellyAnimatorChild != null) gellyAnimatorChild.SetBool(IsMovingHash, false);
            
            onMovementCompleteCallback?.Invoke();
        }
    }
}