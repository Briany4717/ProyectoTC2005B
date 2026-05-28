using UnityEngine;
using UnityEngine.SceneManagement;
using System;


/// Administra la vida del jugador y los eventos relacionados con recibir daño o morir.

public class HOPlayerLives : MonoBehaviour
{
    public int cantidadDeVida;

    public event Action<int> OnLivesChanged;

    
    /// Inicializa y notifica la cantidad de vida inicial.
    
    void Start()
    {
        OnLivesChanged?.Invoke(cantidadDeVida);
    }

    
    /// Resta vida al jugador, actualiza la UI y verifica la condición de derrota.
    
    public void TomarDanio(int danio)
    {
        cantidadDeVida -= danio;
        OnLivesChanged?.Invoke(cantidadDeVida);

        if (cantidadDeVida <= 0)
        {
            if (HOCoins.Instance != null)
            {
                HOCoins.Instance.SaveCoins();
            }
            SceneManager.LoadScene("HOEndScene");
        }
    }
}