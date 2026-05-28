using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LETicTacToeMinigame : MonoBehaviour
{
    [Header("Manager Connection")]
    [SerializeField] private LERepairManager repairManager;

    [Header("UI Canvas Elements")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TextMeshProUGUI announcementTextMesh;
    
    [Tooltip("Arrastra estrictamente los 9 botones del tablero en orden (0-8, de izquierda a derecha, arriba a abajo).")]
    [SerializeField] private Button[] gridButtons;
    [SerializeField] private Image[] gridImages; // Componentes Image de los 9 botones

    [Header("Visual Customization  ")]
    [SerializeField] private Sprite playerXSprite;
    [SerializeField] private Sprite botOSprite;
    [SerializeField] private Sprite emptySprite; // Un sprite transparente o el fondo vacío
    [Tooltip("Tiempo en segundos que el bot 'piensa' antes de tirar.")]
    [SerializeField] private float botThinkingDuration = 0.6f;

    // MATRICES DE CONTROL INTERNO (0 Allocations en Gameloop)
    // 0 = Vacío, 1 = Jugador (X), 2 = Bot (O)
    private readonly int[] boardState = new int[9]; 
    
    // Combinaciones ganadoras estándar indexadas en memoria estática
    private readonly int[,] winIndices = new int[8, 3]
    {
        {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Horizontales
        {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Verticales
        {0, 4, 8}, {2, 4, 6}             // Diagonales
    };

    private bool isPlayerTurn = false;
    private bool isMinigameActive = false;
    private int totalMovesCount = 0;

    void Awake()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }

    /// <summary>
    /// Punto de entrada principal disparado por el LERepairManager.
    /// </summary>
    public void StartMinigame()
    {
        if (minigamePanel != null) minigamePanel.SetActive(true);
        isMinigameActive = true;
        Debug.Log("Entrando a TicTacToe minigame");
        
        ResetBoardData();
        StartCoroutine(DetermineStartingTurnRoutine());
    }

    private void ResetBoardData()
    {
        totalMovesCount = 0;
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = 0;
            if (gridImages[i] != null) gridImages[i].sprite = emptySprite;
            if (gridButtons[i] != null) gridButtons[i].interactable = false; // Bloqueados hasta que pase el banner
        }
    }

    private IEnumerator DetermineStartingTurnRoutine()
    {
        // Decisión aleatoria pura de quién arranca
        isPlayerTurn = Random.Range(0, 2) == 0;

        if (isPlayerTurn)
        {
            announcementTextMesh.text = "¡Inicias tú!";
        }
        else
        {
            announcementTextMesh.text = "¡Inicia el rival!";
        }

        // Retraso controlado inmune a pausas para leer el aviso
        float timer = 0f;
        while (timer < 1.5f)
        {
            if (repairManager.currentState != LERepairManager.RepairState.Paused) timer += Time.deltaTime;
            yield return null;
        }

        SetGridInteractivity(isPlayerTurn);

        if (!isPlayerTurn)
        {
            StartCoroutine(ExecuteBotTurnRoutine());
        }
    }

    /// <summary>
    /// Vincula esta función en el Inspector a los 9 botones usando el ID dinámico correspondiente (0 a 8).
    /// </summary>
    public void OnClickGridCell(int cellIndex)
    {
        if (!isMinigameActive || !isPlayerTurn || boardState[cellIndex] != 0) return;

        // 1. Registrar tiro del jugador
        boardState[cellIndex] = 1;
        if (gridImages[cellIndex] != null) gridImages[cellIndex].sprite = playerXSprite;
        gridButtons[cellIndex].interactable = false;
        totalMovesCount++;

        // 2. Evaluar tablero
        if (CheckWinCondition(1))
        {
            EndMinigame(playerWon: true);
            return;
        }

        if (CheckTieCondition())
        {
            StartCoroutine(HandleTieRoutine());
            return;
        }

        // 3. Pasar turno al bot
        isPlayerTurn = false;
        SetGridInteractivity(false);
        StartCoroutine(ExecuteBotTurnRoutine());
    }

    private IEnumerator ExecuteBotTurnRoutine()
    {
        announcementTextMesh.text = "Gelly pensando...";
        
        float timer = 0f;
        while (timer < botThinkingDuration)
        {
            if (repairManager.currentState != LERepairManager.RepairState.Paused) timer += Time.deltaTime;
            yield return null;
        }

        // ====================================================================
        // 🎛️ SELECTOR DE DIFICULTAD DINÁMICO POR PLAYERPREFS  
        // 0 = Fácil (100% Random), 1 = Medio (50% Inteligente), 2 = Difícil (100% Inteligente)
        // ====================================================================
        int difficulty = PlayerPrefs.GetInt("LE_Minigames_Difficulty", 1); // Por defecto inicia en Medio (1)
        int targetCell = -1;

        if (difficulty == 2)
        {
            // DIFICULTAD DIFÍCIL: Mente maestra impecable
            targetCell = FindCriticalMove(2); // Intentar ganar
            if (targetCell == -1) targetCell = FindCriticalMove(1); // Intentar bloquear
        }
        else if (difficulty == 1)
        {
            // DIFICULTAD MEDIA: El bot tiene un 50% de probabilidad de jugar de forma brillante
            if (Random.value > 0.5f)
            {
                targetCell = FindCriticalMove(2); // Intentar ganar
                if (targetCell == -1) targetCell = FindCriticalMove(1); // Intentar bloquear
            }
        }
        // NOTA: Si es Dificultad Fácil (0), se salta los checks e irá directo al tiro aleatorio inferior

        // Fallback defensivo: Si es Fácil, falló el tiro de dados de la dificultad Media, 
        // o simplemente no hay movimientos de peligro en el tablero, tira de forma aleatoria.
        if (targetCell == -1)
        {
            targetCell = GetRandomEmptyCell();
        }

        // Registrar tiro final del bot
        boardState[targetCell] = 2;
        if (gridImages[targetCell] != null) gridImages[targetCell].sprite = botOSprite;
        gridButtons[targetCell].interactable = false;
        totalMovesCount++;

        if (CheckWinCondition(2))
        {
            EndMinigame(playerWon: false);
            yield break;
        }

        if (CheckTieCondition())
        {
            StartCoroutine(HandleTieRoutine());
            yield break;
        }

        // Devolver turno al jugador de forma fluida
        isPlayerTurn = true;
        announcementTextMesh.text = "¡Tu turno!";
        SetGridInteractivity(true);
    }

    private int FindCriticalMove(int checkTeam)
    {
        for (int i = 0; i < 8; i++)
        {
            int a = winIndices[i, 0];
            int b = winIndices[i, 1];
            int c = winIndices[i, 2];

            if (boardState[a] == checkTeam && boardState[b] == checkTeam && boardState[c] == 0) return c;
            if (boardState[a] == checkTeam && boardState[c] == checkTeam && boardState[b] == 0) return b;
            if (boardState[b] == checkTeam && boardState[c] == checkTeam && boardState[a] == 0) return a;
        }
        return -1;
    }

    private int GetRandomEmptyCell()
    {
        int randomIndex = Random.Range(0, 9);
        while (boardState[randomIndex] != 0)
        {
            randomIndex = Random.Range(0, 9);
        }
        return randomIndex;
    }

    private bool CheckWinCondition(int team)
    {
        for (int i = 0; i < 8; i++)
        {
            if (boardState[winIndices[i, 0]] == team &&
                boardState[winIndices[i, 1]] == team &&
                boardState[winIndices[i, 2]] == team)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckTieCondition() => totalMovesCount >= 9;

    private IEnumerator HandleTieRoutine()
    {
        SetGridInteractivity(false);
        announcementTextMesh.text = "¡Empate! Reiniciando...";
        
        float timer = 0f;
        while (timer < 1.2f)
        {
            if (repairManager.currentState != LERepairManager.RepairState.Paused) timer += Time.deltaTime;
            yield return null;
        }

        ResetBoardData();
        StartCoroutine(DetermineStartingTurnRoutine());
    }

    private void SetGridInteractivity(bool state)
    {
        for (int i = 0; i < gridButtons.Length; i++)
        {
            // Solo se vuelven interactuables las casillas vacías
            if (gridButtons[i] != null)
            {
                gridButtons[i].interactable = state && (boardState[i] == 0);
            }
        }
    }

    private void EndMinigame(bool playerWon)
    {
        isMinigameActive = false;
        SetGridInteractivity(false);

        if (playerWon)
        {
            announcementTextMesh.text = "¡SÍÍ!, ¡GANASTE!";
            StartCoroutine(DelayClosureRoutine(isVictory: true));
        }
        else
        {
            announcementTextMesh.text = "¡OH NO, PERDISTE!";
            StartCoroutine(DelayClosureRoutine(isVictory: false));
        }
    }

    private IEnumerator DelayClosureRoutine(bool isVictory)
    {
        float timer = 0f;
        while (timer < 1.5f)
        {
            if (repairManager.currentState != LERepairManager.RepairState.Paused) timer += Time.deltaTime;
            yield return null;
        }

        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (isVictory)
        {
            repairManager.SimulateWinMinigame();
        }
        else
        {
            repairManager.RegisterMinigameStrikeFailure();
        }
    }
}
