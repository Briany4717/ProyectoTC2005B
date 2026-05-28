using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


/// Controla la pausa del juego y el menú correspondiente.

public class HOPauseController : MonoBehaviour
{
    public GameObject menuPause;
    [SerializeField] HOPlayerControl playerController;
    
    
    /// Detecta la entrada para alternar el estado de pausa.
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    
    /// Alterna entre pausar y continuar el juego.
    
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
    
    
    /// Pausa el juego, detiene el tiempo y desactiva al jugador.
    
    private void PauseGame()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        playerController.enabled = false;
    }
    
    
    /// Reanuda el juego, restaura el tiempo y activa al jugador.
    
    private void ContinueGame()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        playerController.enabled = true;
    }
}