using UnityEngine;
using System;

public class HOCoins : MonoBehaviour
{
    public static HOCoins Instance { get; private set; }

    private int currentCoins = 0;

    // Evento que se dispara cada vez que cambian las monedas
    public event Action<int> OnCoinsChanged;

    // Propiedad pública de solo lectura para consultar las monedas actuales
    public int CurrentCoins => currentCoins;

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
        // Notifica el valor inicial al arrancar (útil para que la UI arranque en 0)
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void ResetCoins()
    {
        currentCoins = 0;
        OnCoinsChanged?.Invoke(currentCoins);
    }
}