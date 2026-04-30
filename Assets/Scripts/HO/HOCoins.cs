using UnityEngine;
using System;

public class HOCoins : MonoBehaviour
{
    public static HOCoins Instance { get; private set; }

    private int currentCoins = 0;

    // Evento para cada vez que cambian las monedas
    public event Action<int> OnCoinsChanged;

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
    
    public void SaveCoins()
    {
        PlayerPrefs.SetInt("LastGameCoins", currentCoins);
        PlayerPrefs.Save();
    }
}