using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private AnimationCurve horizontalCurve; // Controla la aceleración/frenado en el suelo
    [SerializeField] private float duration = 0.6f;

    [Header("Jump Math (Foolproof)")]
    [SerializeField] private float maxJumpHeight = 2.0f;  // Altura máxima del arco

    [Header("References")]
    public Transform gellyTransformParent;
    public Animator gellyAnimatorChild;

    private float timer = 0f;
    private bool isJumping = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private System.Action onJumpCompleteCallback;

    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");

    public void JumpTo(Vector2 targetPos, System.Action onComplete = null)
    {
        if (isJumping) return;

        startPos = gellyTransformParent.position;
        endPos = new Vector3(targetPos.x, targetPos.y, startPos.z);
        onJumpCompleteCallback = onComplete;
        
        timer = 0f;
        isJumping = true;

        if (gellyAnimatorChild != null)
        {
            gellyAnimatorChild.SetTrigger(JumpTriggerHash);
        }
    }

    void Update()
    {
        if (!isJumping) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // 1. Movimiento horizontal guiado por tu curva elástica
        float hProgress = horizontalCurve.Evaluate(t);
        Vector3 groundPos = Vector3.LerpUnclamped(startPos, endPos, hProgress);

        // 2. Parábola matemática pura: 4 * t * (1 - t). 
        // Siempre da 0 al inicio, 1 al centro y 0 al final. ¡Física perfecta sin fallas!
        float jumpProgress = 4f * t * (1f - t); 
        float currentHeight = jumpProgress * maxJumpHeight;

        // 3. Aplicamos la combinación al Padre
        gellyTransformParent.position = new Vector3(groundPos.x, groundPos.y + currentHeight, groundPos.z);

        if (t >= 1f)
        {
            isJumping = false;
            onJumpCompleteCallback?.Invoke();
        }
    }
}