using UnityEngine;
using System;


/// Controla la lógica del evento "Super Prompt", gestionando el progreso y las recompensas/penalizaciones.

public class HOSuperPrompt : MonoBehaviour
{
    public static HOSuperPrompt Instance { get; private set; }

    public int enemiesRequired = 4;
    [Range(0f, 1f)]
    public float difficultyReductionPercent = 0.5f;

    private int enemiesDefeated = 0;
    private bool isPromptActive = false;

    public event Action<int, int> OnProgressChanged; 
    public event Action OnPromptTriggered;
    public event Action OnPromptClosed;

    public int EnemiesDefeated => enemiesDefeated;
    public int EnemiesRequired => enemiesRequired;
    public bool IsPromptActive => isPromptActive;

    
    /// Configura el singleton de la clase.
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// Notifica el progreso inicial.
    
    void Start()
    {
        OnProgressChanged?.Invoke(enemiesDefeated, enemiesRequired);
    }

    
    /// Incrementa el contador de enemigos derrotados y activa el prompt si se alcanza el objetivo.
    
    public void OnEnemyDefeated()
    {
        if (isPromptActive) return;

        enemiesDefeated++;
        OnProgressChanged?.Invoke(enemiesDefeated, enemiesRequired);

        if (enemiesDefeated >= enemiesRequired)
        {
            TriggerPrompt();
        }
    }

    
    /// Pausa el juego y muestra el prompt.
    
    void TriggerPrompt()
    {
        isPromptActive = true;
        Time.timeScale = 0f;
        OnPromptTriggered?.Invoke();
    }

    
    /// Aplica las recompensas y cierra el prompt al responder correctamente.
    
    public void OnAnswerCorrect()
    {
        ApplyDifficultyReduction();
        ClosePrompt(resetCounter: true);
    }

    
    /// Reduce la dificultad disminuyendo la velocidad de la cámara y el nivel de los generadores.
    
    void ApplyDifficultyReduction()
    {
        if (HOScrollingCamera.Instance != null)
        {
            HOScrollingCamera.Instance.ReduceDifficulty(difficultyReductionPercent);
        }

        HOEnemySpawner[] spawners = FindObjectsByType<HOEnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.ReduceDifficulty(difficultyReductionPercent);
        }
    }

    
    /// Cierra el prompt sin aplicar recompensas si la respuesta es incorrecta.
    
    public void OnAnswerIncorrect()
    {
        ClosePrompt(resetCounter: true);
    }

    
    /// Reanuda el juego y reinicia el progreso si es necesario.
    
    void ClosePrompt(bool resetCounter)
    {
        isPromptActive = false;
        Time.timeScale = 1f;

        if (resetCounter)
        {
            enemiesDefeated = 0;
            OnProgressChanged?.Invoke(enemiesDefeated, enemiesRequired);
        }

        OnPromptClosed?.Invoke();
    }
}