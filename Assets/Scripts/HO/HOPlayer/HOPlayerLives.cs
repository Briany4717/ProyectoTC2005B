using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class HOPlayerLives : MonoBehaviour
{
    public int cantidadDeVida;


    public event Action<int> OnLivesChanged;

    void Start()
    {
        OnLivesChanged?.Invoke(cantidadDeVida);
    }

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
