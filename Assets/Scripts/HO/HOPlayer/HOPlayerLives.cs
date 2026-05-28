using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Administra la vida del jugador y los eventos relacionados con recibir daño o morir.
/// </summary>
public class HOPlayerLives : MonoBehaviour
{
    public int cantidadDeVida;

    public event Action<int> OnLivesChanged;

    /// <summary>
    /// Inicializa y notifica la cantidad de vida inicial.
    /// </summary>
    void Start()
    {
        OnLivesChanged?.Invoke(cantidadDeVida);
    }

    /// <summary>
    /// Resta vida al jugador, actualiza la UI y verifica la condición de derrota.
    /// </summary>
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