using System.Collections;
using UnityEngine;

public class LEStartSceneController : MonoBehaviour
{
    [Header("UI Menus")]
    public GameObject playMenu;
    public GameObject startMenu;
    public GameObject tutorialPanel;
    public LETutorialManager tutorialController;
    
    // ¡REINTEGRADO!: El contenedor principal de toda la UI de juego de la cinta (⌐■_■)
    [Tooltip("Arrastra aquí el panel principal que contiene el HUD y elementos del juego.")]
    public GameObject gamePanel; 

    [Header("Box Visuals")]
    public SpriteRenderer boxObject;
    public Sprite boxOpenSprite;
    public Sprite closeBoxSprite;
    public bool boxIsMoving;

    [Header("Phase 1: Idle Shake Settings")]
    [SerializeField] private float idleIntensity = 0.05f;
    [SerializeField] private float idleSpeed = 25f;
    [SerializeField] private float idleDuration = 0.4f;
    [SerializeField] private float idleInterval = 2.5f; 

    [Header("Phase 2: Gacha Burst Settings")]
    [SerializeField] private float burstDuration = 1.8f;   
    [Tooltip("Duración corta para el inicio directo sin tutorial.")]
    [SerializeField] private float directPlayBurstDuration = 0.45f; 
    [SerializeField] private float maxBurstIntensity = 0.35f; 
    [SerializeField] private float maxBurstSpeed = 60f;

    [Header("Direct Mode Configuration")]
    [SerializeField] private LEGellyCharacterController gellyCharacter; 
    [Tooltip("Posición final exacta en coordenadas del mundo donde caerá Gelly.")]
    [SerializeField] private Vector2 directTargetPosition; 
    [Tooltip("Escala final que adoptará el personaje al aterrizar.")]
    [SerializeField] private float directCharacterTargetScale = 1.0f; 
    [SerializeField] private LENetworkManager networkManager;
    [SerializeField] private LEConveyorManager conveyorManager; 

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private WaitForSeconds cachedIdleWait;

    void Start()
    {
        if (LEGameSessionData.Instance.isGameInProgress)
        {
            if (playMenu != null) playMenu.SetActive(false);
            if (startMenu != null) startMenu.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (boxObject != null) boxObject.gameObject.SetActive(false); 
            if (gamePanel != null) gamePanel.SetActive(true);

            if (conveyorManager != null)
            {
                conveyorManager.gameObject.SetActive(true);
                conveyorManager.InitializeConveyorGameplay();
            }
            return;
        }

        LEGameSessionData.Instance.ResetSession(burstDuration); 
    
        if (networkManager != null) networkManager.FetchMatchData();
        if (boxObject != null)
        {
            originalPosition = boxObject.transform.position;
            originalRotation = boxObject.transform.rotation;
            boxObject.sprite = closeBoxSprite;
        }

        cachedIdleWait = new WaitForSeconds(idleInterval);
        boxIsMoving = true;
        StartCoroutine(IdleShakeRoutine());
    }

    public void StartTutorial()
    {
        if (!boxIsMoving) return; 
        boxIsMoving = false; 

        playMenu.SetActive(false);
        startMenu.SetActive(false);
        tutorialPanel.SetActive(true);

        StartCoroutine(GachaBurstRoutine(isTutorialFlow: true));
    }

    public void StartDirectGame()
    {
        if (!boxIsMoving) return;
        boxIsMoving = false;

        playMenu.SetActive(false);
        startMenu.SetActive(false);

        StartCoroutine(GachaBurstRoutine(isTutorialFlow: false));
    }

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
                float offsetX = Mathf.Sin(Time.time * idleSpeed) * idleIntensity;
                boxObject.transform.position = originalPosition + new Vector3(offsetX, 0f, 0f);
                yield return null;
            }
            boxObject.transform.position = originalPosition;
        }
    }

    private IEnumerator GachaBurstRoutine(bool isTutorialFlow)
    {
        float timer = 0f;
        float targetBurstDuration = isTutorialFlow ? burstDuration : directPlayBurstDuration;

        while (timer < targetBurstDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / targetBurstDuration; 
            float curveSmooth = progress * progress; 

            float currentIntensity = Mathf.Lerp(idleIntensity, maxBurstIntensity, curveSmooth);
            float currentSpeed = Mathf.Lerp(idleSpeed, maxBurstSpeed, curveSmooth);

            float offsetX = Mathf.Sin(Time.time * currentSpeed) * currentIntensity;
            float offsetY = Mathf.Abs(Mathf.Cos(Time.time * currentSpeed)) * (currentIntensity * 0.4f);
            float offsetRotationZ = Mathf.Sin(Time.time * (currentSpeed * 1.2f)) * (currentIntensity * 30f); 

            boxObject.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);
            boxObject.transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, offsetRotationZ);

            yield return null;
        }

        boxObject.transform.position = originalPosition;
        boxObject.transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, isTutorialFlow ? 40f : 0f);

        if (boxObject != null && boxOpenSprite != null)
        {
            boxObject.sprite = boxOpenSprite;
        }

        yield return new WaitForSeconds(0.25f);

        if (isTutorialFlow)
        {
            tutorialController.StartTutorial();
        }
        else
        {
            if (gellyCharacter != null)
            {
                // Al iniciar normal, también encendemos el gamePanel al aterrizar
                gellyCharacter.JumpVerticalTo(directTargetPosition, directCharacterTargetScale, () => 
                {
                    if (gamePanel != null) gamePanel.SetActive(true);
                    if (conveyorManager != null)
                    {
                        conveyorManager.gameObject.SetActive(true);
                        conveyorManager.InitializeConveyorGameplay();
                    }
                });

                yield return new WaitForSeconds(1f);
                boxObject.gameObject.SetActive(false);
                playMenu.SetActive(true);
            }
        }
    }
}
