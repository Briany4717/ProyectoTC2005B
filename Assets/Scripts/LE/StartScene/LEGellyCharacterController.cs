using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private AnimationCurve horizontalCurve; 
    [SerializeField] private float duration = 0.6f;

    [Header("Jump Math & Timing")]
    [SerializeField] private float maxJumpHeight = 2.0f;  
    [SerializeField] private float anticipationDelay = 0.2f; // El tiempo exacto que tarda el modelo en "tomar vuelo" antes de despegar

    [Header("References")]
    public Transform gellyTransformParent;
    public Animator gellyAnimatorChild;

    private float elapsedTime = 0f;
    private bool isJumping = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private System.Action onJumpCompleteCallback;

    // Hashes cacheados para rendimiento extremo en la CPU
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int LandTriggerHash = Animator.StringToHash("Land");

    public void JumpTo(Vector2 targetPos, System.Action onComplete = null)
    {
        if (isJumping) return;

        startPos = gellyTransformParent.position;
        endPos = new Vector3(targetPos.x, targetPos.y, startPos.z);
        onJumpCompleteCallback = onComplete;
        
        elapsedTime = 0f;
        isJumping = true;

        // Decimos al Animator que empiece a tomar vuelo (Gelly_JumpStart)
        if (gellyAnimatorChild != null)
        {
            gellyAnimatorChild.SetTrigger(JumpTriggerHash);
        }
    }

    void Update()
    {
        if (!isJumping) return;

        elapsedTime += Time.deltaTime;

        // PARTE 1: Fase de Anticipación. El personaje no se mueve del suelo aún.
        if (elapsedTime < anticipationDelay)
        {
            // Mantiene al padre firmemente en el punto de inicio mientras el hijo se deforma visualmente
            gellyTransformParent.position = startPos; 
            return;
        }

        // PARTE 2: Fase de Vuelo (Física y Desplazamiento)
        // Normalizamos el tiempo restando el delay de anticipación
        float jumpTime = elapsedTime - anticipationDelay;
        float t = Mathf.Clamp01(jumpTime / duration);

        // Movimiento horizontal elástico
        float hProgress = horizontalCurve.Evaluate(t);
        Vector3 groundPos = Vector3.LerpUnclamped(startPos, endPos, hProgress);

        // Parábola matemática pura: Garantiza regresar a 0 exactamente en t = 1.0
        float jumpProgress = 4f * t * (1f - t); 
        float currentHeight = jumpProgress * maxJumpHeight;

        // Aplicamos la posición unificada al padre
        gellyTransformParent.position = new Vector3(groundPos.x, groundPos.y + currentHeight, groundPos.z);

        // PARTE 3: Impacto contra el suelo (Aterrizaje)
        if (t >= 1f)
        {
            isJumping = false;

            // Le avisamos al Animator de forma nativa que rompa el bucle del aire y aterrice
            if (gellyAnimatorChild != null)
            {
                gellyAnimatorChild.SetTrigger(LandTriggerHash);
            }

            onJumpCompleteCallback?.Invoke();
        }
    }
}