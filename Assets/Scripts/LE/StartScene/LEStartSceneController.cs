using System.Collections;
using UnityEngine;

public class LEStartSceneController : MonoBehaviour
{
    [Header("UI Menus")]
    public GameObject playMenu;
    public GameObject startMenu;
    public GameObject tutorialPanel;
    public LETutorialManager tutorialController;

    [Header("Box Visuals")]
    public SpriteRenderer boxObject;
    public Sprite boxOpenSprite;
    public Sprite closeBoxSprite;
    public bool boxIsMoving;

    [Header("Phase 1: Idle Shake Settings")]
    [SerializeField] private float idleIntensity = 0.05f;
    [SerializeField] private float idleSpeed = 25f;
    [SerializeField] private float idleDuration = 0.4f;
    [SerializeField] private float idleInterval = 2.5f; // Tiempo de espera entre pequeños temblores de misterio

    [Header("Phase 2: Gacha Burst Settings")]
    [SerializeField] private float burstDuration = 1.8f;   // Cuánto tiempo se agita antes de estallar
    [SerializeField] private float maxBurstIntensity = 0.35f; 
    [SerializeField] private float maxBurstSpeed = 60f;

    // Cache de transformación absoluta para evitar desvíos de coordenadas
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private WaitForSeconds cachedIdleWait;

    void Start()
    {
        if (boxObject != null)
        {
            originalPosition = boxObject.transform.position;
            originalRotation = boxObject.transform.rotation;
            boxObject.sprite = closeBoxSprite;
        }

        // Optimización quirúrgica: Cacheamos el WaitForSeconds para tener ZERO Allocations en el loop
        cachedIdleWait = new WaitForSeconds(idleInterval);

        boxIsMoving = true;
        StartCoroutine(IdleShakeRoutine());
    }

    /// <summary>
    /// Vincula esta función directamente al botón de Unity para iniciar.
    /// </summary>
    public void StartTutorial()
    {
        // Seguridad: Si ya se presionó, evitamos doble ejecución
        if (!boxIsMoving) return; 
        
        boxIsMoving = false; // Detiene el bucle pasivo del menú

        // Apagamos los menús de inmediato para enfocar toda la atención en la caja estallando
        playMenu.SetActive(false);
        startMenu.SetActive(false);
        tutorialPanel.SetActive(true);

        // Iniciamos la secuencia de gacha incremental
        StartCoroutine(GachaBurstRoutine());
    }

    /// <summary>
    /// FASE 1: Pequeños temblores misteriosos en el menú principal para indicar que hay vida dentro.
    /// </summary>
    private IEnumerator IdleShakeRoutine()
    {
        while (boxIsMoving)
        {
            yield return cachedIdleWait;

            if (!boxIsMoving) break;

            float timer = 0f;
            while (timer < idleDuration && boxIsMoving)
            {
                timer += Time.deltaTime;
                
                // Ondas senoidales rápidas para un temblor sutil en X
                float offsetX = Mathf.Sin(Time.time * idleSpeed) * idleIntensity;
                boxObject.transform.position = originalPosition + new Vector3(offsetX, 0f, 0f);
                
                yield return null;
            }

            // Regresa a su estado base perfectamente estable
            boxObject.transform.position = originalPosition;
        }
    }

    /// <summary>
    /// FASE 2: Animación incremental dramática. Se agita más rápido y fuerte hasta explotar.
    /// </summary>
    private IEnumerator GachaBurstRoutine()
    {
        float timer = 0f;

        while (timer < burstDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / burstDuration; // Va de 0.0 a 1.0 de forma lineal

            // Curva de aceleración exponencial para el sentimiento de "Sorpresa Gacha"
            float curveSmooth = progress * progress; 

            // Intensidad y velocidad escalan dinámicamente con el tiempo
            float currentIntensity = Mathf.Lerp(idleIntensity, maxBurstIntensity, curveSmooth);
            float currentSpeed = Mathf.Lerp(idleSpeed, maxBurstSpeed, curveSmooth);

            // Matemática de oscilación: X se agita, Y salta un poco, Z rota elásticamente
            float offsetX = Mathf.Sin(Time.time * currentSpeed) * currentIntensity;
            float offsetY = Mathf.Abs(Mathf.Cos(Time.time * currentSpeed)) * (currentIntensity * 0.4f);
            float offsetRotationZ = Mathf.Sin(Time.time * (currentSpeed * 1.2f)) * (currentIntensity * 30f); // Balanceo de la caja

            // Aplicamos transformaciones al SpriteRenderer directamente
            boxObject.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);
            boxObject.transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, offsetRotationZ);

            yield return null;
        }

        // ¡BOOM! Restablecemos coordenadas físicas a la perfección
        boxObject.transform.position = originalPosition;
        boxObject.transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, 40f);

        // Cambiamos el sprite al de la caja abierta de forma nativa
        if (boxObject != null && boxOpenSprite != null)
        {
            boxObject.sprite = boxOpenSprite;
        }

        // Un pequeño golpe de retraso (0.25s) para el impacto visual antes de abrir los diálogos
        yield return new WaitForSeconds(0.25f);

        // Activamos la UI del tutorial e iniciamos tu orquestador de datos
        tutorialController.StartTutorial();
    }
}
