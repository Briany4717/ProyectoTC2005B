using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private AnimationCurve horizontalCurve; // Una curva suave de inicio a fin (0 a 1)
    [SerializeField] private float duration = 0.6f;

    [Header("Jump Arc (Vertical)")]
    [SerializeField] private AnimationCurve jumpArcCurve; // Una curva con forma de parábola (sube a 1 en el centro y baja a 0)
    [SerializeField] private float maxJumpHeight = 2.0f;  // Qué tan alto saltará el personaje

    [Header("References")]
    public Transform gellyTransformParent;
    public Animator gellyAnimatorChild;

    private float timer = 0f;
    private bool isJumping = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private System.Action onJumpCompleteCallback;

    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump"); // O el nombre de tu Trigger de animación

    public void JumpTo(Vector2 targetPos, System.Action onComplete = null)
    {
        if (isJumping) return;

        startPos = gellyTransformParent.position;
        endPos = new Vector3(targetPos.x, targetPos.y, startPos.z);
        onJumpCompleteCallback = onComplete;
        
        timer = 0f;
        isJumping = true;

        // Despierta la animación de salto en el hijo inmediatamente
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

        // 1. Calculamos la posición base en el suelo (Lerp lineal guiado por la curva de velocidad)
        float hProgress = horizontalCurve.Evaluate(t);
        Vector3 groundPos = Vector3.LerpUnclamped(startPos, endPos, hProgress);

        // 2. Calculamos la altura del salto de forma independiente
        float vProgress = jumpArcCurve.Evaluate(t);
        float currentHeight = vProgress * maxJumpHeight;

        // 3. Combinamos ambos cálculos en el transform del Padre
        gellyTransformParent.position = new Vector3(groundPos.x, groundPos.y + currentHeight, groundPos.z);

        if (t >= 1f)
        {
            isJumping = false;
            onJumpCompleteCallback?.Invoke();
        }
    }
}
