using UnityEngine;

public class LEGellyCharacterController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private AnimationCurve horizontalCurve; 
    [SerializeField] private float duration = 0.6f;

    [Header("Jump Math & Timing")]
    [SerializeField] private float maxJumpHeight = 2.0f;  
    [SerializeField] private float anticipationDelay = 0.2f; // Tiempo de espera en el suelo antes de despegar

    [Header("Landing Anticipation (Negative Delay)")]
    [Tooltip("Cuántos segundos ANTES de tocar el suelo debe comenzar la animación de aterrizaje.")]
    [SerializeField] private float landingAnticipationTime = 0.12f; 

    [Header("References")]
    public Transform gellyTransformParent;
    public Animator gellyAnimatorChild;

    private float elapsedTime = 0f;
    private bool isJumping = false;
    private bool landTriggered = false; // Flag para evitar disparar el Trigger múltiples veces
    private Vector3 startPos;
    private Vector3 endPos;
    private System.Action onJumpCompleteCallback;

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
        landTriggered = false; // Reiniciamos el flag para el nuevo salto

        if (gellyAnimatorChild != null)
        {
            gellyAnimatorChild.SetTrigger(JumpTriggerHash);
        }
    }

    void Update()
    {
        if (!isJumping) return;

        elapsedTime += Time.deltaTime;

        // PARTE 1: Fase de Anticipación (Wind-up)
        if (elapsedTime < anticipationDelay)
        {
            gellyTransformParent.position = startPos; 
            return;
        }

        // PARTE 2: Fase de Vuelo
        float jumpTime = elapsedTime - anticipationDelay;
        float t = Mathf.Clamp01(jumpTime / duration);

        // Movimiento horizontal elástico
        float hProgress = horizontalCurve.Evaluate(t);
        Vector3 groundPos = Vector3.LerpUnclamped(startPos, endPos, hProgress);

        // Parábola matemática pura
        float jumpProgress = 4f * t * (1f - t); 
        float currentHeight = jumpProgress * maxJumpHeight;

        gellyTransformParent.position = new Vector3(groundPos.x, groundPos.y + currentHeight, groundPos.z);

        // ==========================================
        // TRUCO PRO: El Delay Negativo de Aterrizaje
        // ==========================================
        float remainingFlightTime = duration - jumpTime;

        if (remainingFlightTime <= landingAnticipationTime && !landTriggered)
        {
            landTriggered = true; // Nos aseguramos de ejecutar esto una sola vez
            
            if (gellyAnimatorChild != null)
            {
                gellyAnimatorChild.SetTrigger(LandTriggerHash);
            }
        }
        // ==========================================

        // PARTE 3: Fin del arco (Impacto en suelo)
        if (t >= 1f)
        {
            isJumping = false;

            // Seguridad por si el landingAnticipationTime se configuró en 0
            if (!landTriggered && gellyAnimatorChild != null)
            {
                gellyAnimatorChild.SetTrigger(LandTriggerHash);
            }

            onJumpCompleteCallback?.Invoke();
        }
    }
}