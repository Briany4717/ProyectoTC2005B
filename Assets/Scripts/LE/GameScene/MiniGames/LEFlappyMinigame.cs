using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class LEFlappyMinigame : MonoBehaviour
{
    [Header("Manager Connection")]
    [SerializeField] private LERepairManager repairManager;

    [Header("UI Canvas Panels")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TextMeshProUGUI announcementTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    [Header("Specific Window Bounds")]
    [Tooltip("Arrastra aquí el RectTransform del recuadro específico (Viewport) donde se debe jugar.")]
    [SerializeField] private RectTransform gameViewport; 

    [Header("The Bird Element")]
    [SerializeField] private RectTransform birdRectTransform;
    [SerializeField] private float gravityForce = 1200f;
    [SerializeField] private float jumpImpulse = 380f;

    [Header("Infinite Recycled Pipes")]
    [Tooltip("Arrastra los 2 contenedores de las parejas de tubos.")]
    [SerializeField] private RectTransform[] pipePairs; 
    [Tooltip("Punto X inicial relativo al recuadro para spawnear.")]
    [SerializeField] private float spawnPointX = 350f;
    [Tooltip("Punto X relativo al recuadro donde el tubo debe reciclarse.")]
    [SerializeField] private float recyclePointX = -350f;
    [Tooltip("Separación horizontal matemática estricta entre tubos.")]
    [SerializeField] private float horizontalSeparation = 400f;
    [SerializeField] private Animator flappyAnimator;

    private float currentPipeSpeed = 250f;
    private float currentVerticalGap = 140f;
    private int targetScoreToWin = 5;

    private float birdVerticalVelocity = 0f;
    private int currentScore = 0;
    private bool isMinigameActive = false;
    
    // ¡EL CENTINELA DE ENGRANAJE! (⌐■_■)
    private bool hasStarted = false; 
    
    private float initialBirdY;
    private float viewportHeight; 

    void Awake()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (birdRectTransform != null) initialBirdY = birdRectTransform.anchoredPosition.y;
        
        CalculateViewportDimensions();
    }

    public void StartMinigame()
    {
        if (minigamePanel != null) minigamePanel.SetActive(true);
        
        CalculateViewportDimensions();
        ConfigureDifficulty();
        ResetFlappyGame();
    }

    private void CalculateViewportDimensions()
    {
        if (gameViewport != null)
        {
            viewportHeight = gameViewport.rect.height;
        }
        else
        {
            viewportHeight = ((RectTransform)transform).rect.height;
        }
    }

    private void ConfigureDifficulty()
    {
        int difficulty = PlayerPrefs.GetInt("LE_Minigames_Difficulty", 1);

        if (difficulty == 0) 
        {
            currentPipeSpeed = 180f;
            currentVerticalGap = 170f;
            targetScoreToWin = 3;
        }
        else if (difficulty == 1) 
        {
            currentPipeSpeed = 250f;
            currentVerticalGap = 135f;
            targetScoreToWin = 5;
        }
        else if (difficulty == 2) 
        {
            currentPipeSpeed = 340f;
            currentVerticalGap = 105f; 
            targetScoreToWin = 7;
        }

        if (scoreTextMesh != null) scoreTextMesh.text = $"0 / {targetScoreToWin}";
    }

    private void ResetFlappyGame()
    {
        currentScore = 0;
        birdVerticalVelocity = 0f;
        
        // El centinela inicia abajo: El mundo físico se congela temporalmente (o^^)o
        hasStarted = false; 

        if (birdRectTransform != null) 
            birdRectTransform.anchoredPosition = new Vector2(birdRectTransform.anchoredPosition.x, initialBirdY);

        for (int i = 0; i < pipePairs.Length; i++)
        {
            if (pipePairs[i] != null)
            {
                float startingX = spawnPointX + (i * horizontalSeparation);
                pipePairs[i].anchoredPosition = new Vector2(startingX, 0f);
                ResizeAndPositionPipeGaps(pipePairs[i]);
            }
        }

        isMinigameActive = true;
        
        if (announcementTextMesh != null) 
            announcementTextMesh.text = "<color=#FFFF00>¡DA CLICK PARA EMPEZAR!</color>";
    }

    void Update()
    {
        if (!isMinigameActive || repairManager.currentState == LERepairManager.RepairState.Paused) return;

        // ====================================================================
        // FASE A: ESPERA DE GATILLO DE INICIO (Ready State) (⌐■_■)
        // El ave y los tubos no se mueven hasta que el puntero haga click por primera vez.
        // ====================================================================
        if (!hasStarted)
        {
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                hasStarted = true;
                flappyAnimator.SetTrigger("Flutter");
                birdVerticalVelocity = jumpImpulse; // Rompemos la inercia con el primer salto interactivo

                if (announcementTextMesh != null) 
                    announcementTextMesh.text = ""; // Limpiamos el letrero de inicio de inmediato
            }
            return; // Abortamos el frame aquí para que las simulaciones sigan congeladas
        }

        // ====================================================================
        // FASE B: GAMEPLAY ACTIVO EN CURSO
        // Captura el input para los saltos subsecuentes del juego de forma fluida.
        // ====================================================================
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            flappyAnimator.SetTrigger("Flutter");
            birdVerticalVelocity = jumpImpulse;
        }

        HandleBirdPhysics();
        HandlePipesMovementAndRecycling();
        ExecuteCollisionMatrixCheck();
    }

    private void HandleBirdPhysics()
    {
        birdVerticalVelocity -= gravityForce * Time.deltaTime;

        Vector2 currentPos = birdRectTransform.anchoredPosition;
        currentPos.y += birdVerticalVelocity * Time.deltaTime;
        birdRectTransform.anchoredPosition = currentPos;

        float halfHeight = viewportHeight * 0.5f;
        if (currentPos.y > halfHeight || currentPos.y < -halfHeight)
        {
            TriggerGameOverCollision();
        }
    }

    private void HandlePipesMovementAndRecycling()
    {
        for (int i = 0; i < pipePairs.Length; i++)
        {
            RectTransform pipePair = pipePairs[i];
            Vector2 pos = pipePair.anchoredPosition;

            pos.x -= currentPipeSpeed * Time.deltaTime;
            pipePair.anchoredPosition = pos;

            if (pos.x <= recyclePointX)
            {
                pos.x += pipePairs.Length * horizontalSeparation;
                pipePair.anchoredPosition = pos;

                ResizeAndPositionPipeGaps(pipePair);

                currentScore++;
                if (scoreTextMesh != null) scoreTextMesh.text = $"{currentScore} / {targetScoreToWin}";

                if (currentScore >= targetScoreToWin)
                {
                    TriggerVictoryConclusion();
                    return;
                }
            }
        }
    }

    private void ResizeAndPositionPipeGaps(RectTransform pipePair)
    {
        float maxVariance = (viewportHeight * 0.5f) - (currentVerticalGap * 0.5f) - 20f;
        if (maxVariance < 10f) maxVariance = 10f; 
        
        float gapCenterY = Random.Range(-maxVariance, maxVariance);

        RectTransform topPipe = pipePair.GetChild(0) as RectTransform;
        RectTransform bottomPipe = pipePair.GetChild(1) as RectTransform;

        float halfViewport = viewportHeight * 0.5f;

        if (topPipe != null)
        {
            float topHeight = halfViewport - gapCenterY - (currentVerticalGap * 0.5f);
            topPipe.sizeDelta = new Vector2(topPipe.sizeDelta.x, topHeight);
            topPipe.anchoredPosition = new Vector2(0f, halfViewport - (topHeight * 0.5f));
        }

        if (bottomPipe != null)
        {
            float bottomHeight = halfViewport + gapCenterY - (currentVerticalGap * 0.5f);
            bottomPipe.sizeDelta = new Vector2(bottomPipe.sizeDelta.x, bottomHeight);
            bottomPipe.anchoredPosition = new Vector2(0f, -halfViewport + (bottomHeight * 0.5f));
        }
    }

    private void ExecuteCollisionMatrixCheck()
    {
        for (int i = 0; i < pipePairs.Length; i++)
        {
            Transform pairTransform = pipePairs[i];
            for (int childIndex = 0; childIndex < 2; childIndex++)
            {
                RectTransform pipeChild = pairTransform.GetChild(childIndex) as RectTransform;
                if (pipeChild != null && CheckRectOverlapAABB(birdRectTransform, pipeChild))
                {
                    TriggerGameOverCollision();
                    return;
                }
            }
        }
    }

    private bool CheckRectOverlapAABB(RectTransform rectA, RectTransform rectB)
    {
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        
        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);

        return (cornersA[2].x > cornersB[0].x && cornersA[0].x < cornersB[2].x &&
                cornersA[2].y > cornersB[0].y && cornersA[0].y < cornersB[2].y);
    }

    private void TriggerGameOverCollision()
    {
        isMinigameActive = false;
        announcementTextMesh.text = "<color=#FF5449>¡IMPACTO DETECTADO!</color>";
        StartCoroutine(EndMinigameRoutine(won: false));
    }

    private void TriggerVictoryConclusion()
    {
        isMinigameActive = false;
        announcementTextMesh.text = "¡COMPLETADO!";
        StartCoroutine(EndMinigameRoutine(won: true));
    }

    private IEnumerator EndMinigameRoutine(bool won)
    {
        yield return new WaitForSeconds(1.3f);
        minigamePanel.SetActive(false);

        if (won) repairManager.SimulateWinMinigame();
        else repairManager.RegisterMinigameStrikeFailure();
    }
}
