using UnityEngine;
using System;

public class HOSuperPrompt : MonoBehaviour
{
    public static HOSuperPrompt Instance { get; private set; }

    public int enemiesRequired = 4;

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
        // Notifica el estado inicial para que la UI arranque correctamente
        OnProgressChanged?.Invoke(enemiesDefeated, enemiesRequired);
    }

    /// <summary>
    /// Lo llama HOEnemyLives cuando un enemigo muere.
    /// </summary>
    public void OnEnemyDefeated()
    {
        // Si el prompt ya está activo, no contar más enemigos
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

    /// <summary>
    /// Lo llama el panel cuando el usuario responde correctamente.
    /// </summary>
    public void OnAnswerCorrect()
    {
        // Aquí más adelante invocaremos la reducción de dificultad
        // Por ahora solo cerramos el prompt
        ClosePrompt(resetCounter: true);
    }

    /// <summary>
    /// Lo llama el panel cuando el usuario responde incorrectamente.
    /// </summary>
    public void OnAnswerIncorrect()
    {
        // Cierra el panel y reinicia el contador (sin reducir dificultad)
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