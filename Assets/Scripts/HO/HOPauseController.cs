using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla la pausa del juego y el menú correspondiente.
/// </summary>
public class HOPauseController : MonoBehaviour
{
    public GameObject menuPause;
    [SerializeField] HOPlayerControl playerController;
    
    /// <summary>
    /// Detecta la entrada para alternar el estado de pausa.
    /// </summary>
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Alterna entre pausar y continuar el juego.
    /// </summary>
    public void TogglePause()
    {
        if (menuPause.activeSelf)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    /// <summary>
    /// Pausa el juego, detiene el tiempo y desactiva al jugador.
    /// </summary>
    private void PauseGame()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        playerController.enabled = false;
    }
    
    /// <summary>
    /// Reanuda el juego, restaura el tiempo y activa al jugador.
    /// </summary>
    private void ContinueGame()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        playerController.enabled = true;
    }
}