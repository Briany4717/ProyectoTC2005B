using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


/// Controla la pausa del juego, deteniendo el tiempo y mostrando el menú de pausa.

public class PauseController : MonoBehaviour
{
    public GameObject menuPause;
    [SerializeField] GLPlayerController playerController;

    
    /// Comprueba si se ha presionado la tecla Escape para alternar la pausa.
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            print("esc was pressed!");
            TogglePause();
        }
    }

    
    /// Alterna entre el estado de juego pausado y en curso.
    
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

    
    /// Pausa el juego, activando el menú y deshabilitando el control del jugador.
    
    private void PauseGame()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        playerController.enabled = false;
    }

    
    /// Reanuda el juego, ocultando el menú y restaurando el control del jugador.
    
    private void ContinueGame()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        playerController.enabled = true;
    }
}