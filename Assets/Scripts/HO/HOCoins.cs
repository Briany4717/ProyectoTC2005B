using UnityEngine;
using System;

/// <summary>
/// Maneja la recolección, almacenamiento y eventos de las monedas del jugador.
/// </summary>
public class HOCoins : MonoBehaviour
{
    public static HOCoins Instance { get; private set; }

    private int currentCoins = 0;

    public event Action<int> OnCoinsChanged;

    public int CurrentCoins => currentCoins;

    /// <summary>
    /// Configura el singleton de la clase.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Invoca el evento inicial de monedas al iniciar.
    /// </summary>
    void Start()
    {
        OnCoinsChanged?.Invoke(currentCoins);
    }

    /// <summary>
    /// Añade monedas y notifica a los suscriptores.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    /// <summary>
    /// Reinicia el contador de monedas a cero.
    /// </summary>
    public void ResetCoins()
    {
        currentCoins = 0;
        OnCoinsChanged?.Invoke(currentCoins);
    }
    
    /// <summary>
    /// Guarda la cantidad de monedas actuales en las preferencias del jugador.
    /// </summary>
    public void SaveCoins()
    {
        PlayerPrefs.SetInt("LastGameCoins", currentCoins);
        PlayerPrefs.Save();
    }
}