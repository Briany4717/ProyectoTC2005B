using UnityEngine;
using System;

public class HOSuperPrompt : MonoBehaviour
{
    public static HOSuperPrompt Instance { get; private set; }

    public int enemiesRequired = 4;
    [Range(0f, 1f)]
    public float difficultyReductionPercent = 0.5f;

    private int enemiesDefeated = 0;
    private bool isPromptActive = false;

    // Eventos para que la UI y el panel se enteren
    public event Action<int, int> OnProgressChanged; // (actual, requerido)
    public event Action OnPromptTriggered;
    public event Action OnPromptClosed;

    // Propiedades de lectura
    public int EnemiesDefeated => enemiesDefeated;
    public int EnemiesRequired => enemiesRequired;
    public bool IsPromptActive => isPromptActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        OnProgressChanged?.Invoke(enemiesDefeated, enemiesRequired);
    }

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

    void TriggerPrompt()
    {
        isPromptActive = true;
        Time.timeScale = 0f;
        OnPromptTriggered?.Invoke();
    }

    public void OnAnswerCorrect()
    {
        ApplyDifficultyReduction();
        ClosePrompt(resetCounter: true);
    }

    void ApplyDifficultyReduction()
    {
        // Reduce velocidad de cámara
        if (HOScrollingCamera.Instance != null)
        {
            HOScrollingCamera.Instance.ReduceDifficulty(difficultyReductionPercent);
        }

        // Reduce nivel de todos los spawners de enemigos
        HOEnemySpawner[] spawners = FindObjectsByType<HOEnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.ReduceDifficulty(difficultyReductionPercent);
        }
    }

    public void OnAnswerIncorrect()
    {
        ClosePrompt(resetCounter: true);
    }

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