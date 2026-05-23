using UnityEngine;

public class LETutorialController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve bezierCurve;
    [SerializeField] private float duration = 1.0f;
    
    [Header("References")]
    public Transform gellyMove;      // El objeto PADRE que moverá posición y escala
    public Animator gellyAnimator;   // El HIJO que solo corre las animaciones base

    [Header("Positions (Tuples)")]
    public (float x, float y) actualPosition;
    public (float x, float y) nextPosition;

    private float timer = 0f;
    private bool isAnimating = false;
    
    // Vectores de trabajo cacheados para evitar allocs en el Update
    private Vector3 startPosVector;
    private Vector3 endPosVector;

    // Cacheamos el hash del Animator para rendimiento de CPU quirúrgico
    private static readonly int IsMovingHash = Animator.StringToHash("Jump");

    public void StartTutorial()
    {
        // Evita reiniciar la animación si ya está corriendo
        if (isAnimating) return; 

        actualPosition = (0.0f,0.0f);

        // Convertimos tus tuplas a Vector3 una sola vez aquí
        startPosVector = new Vector3(actualPosition.x, actualPosition.y, gellyMove.position.z);
        endPosVector = new Vector3(nextPosition.x, nextPosition.y, gellyMove.position.z);
        
        timer = 0f;
        isAnimating = true;

        // Avisamos al Animator del hijo que empiece su ciclo visual
        if (gellyAnimator != null)
        {
            gellyAnimator.SetTrigger(IsMovingHash);
        }
    }

    void Update()
    {
        // DX & Rendimiento: Si no está animando, el Update aborta inmediatamente. ¡Cero costo!
        if (!isAnimating) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // Evaluamos la curva de Bézier (Calculado en el core de C++ nativo de Unity)
        float curveValue = bezierCurve.Evaluate(t); 

        // 1. POSICIÓN: LerpUnclamped permite que la curva guíe el movimiento, incluso con rebotes
        gellyMove.position = Vector3.LerpUnclamped(startPosVector, endPosVector, curveValue);

        // 2. ESCALA: Modificación uniforme en el Padre para una DX limpia y sin estirar el mesh
        gellyMove.localScale = Vector3.one * curveValue;

        // Condición de finalización
        if (t >= 1f)
        {
            isAnimating = false;
            actualPosition = nextPosition; // Sincronizamos tus tuplas al destino final
            
            // Apagamos la animación del hijo de forma fluida
            if (gellyAnimator != null)
            {
                gellyAnimator.SetBool(IsMovingHash, false);
            }
        }
    }
}