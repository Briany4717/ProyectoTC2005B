using UnityEngine;
using System;


/// Maneja la recolección, almacenamiento y eventos de las monedas del jugador.

public class HOCoins : MonoBehaviour
{
    public static HOCoins Instance { get; private set; }

    private int currentCoins = 0;

    public event Action<int> OnCoinsChanged;

    public int CurrentCoins => currentCoins;

    
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

    
    /// Invoca el evento inicial de monedas al iniciar.
    
    void Start()
    {
        OnCoinsChanged?.Invoke(currentCoins);
    }

    
    /// Añade monedas y notifica a los suscriptores.
    
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    
    /// Reinicia el contador de monedas a cero.
    
    public void ResetCoins()
    {
        currentCoins = 0;
        OnCoinsChanged?.Invoke(currentCoins);
    }
    
    
    /// Guarda la cantidad de monedas actuales en las preferencias del jugador.
    
    public void SaveCoins()
    {
        PlayerPrefs.SetInt("LastGameCoins", currentCoins);
        PlayerPrefs.Save();
    }
}