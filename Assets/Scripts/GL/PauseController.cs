using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla la pausa del juego, deteniendo el tiempo y mostrando el menú de pausa.
/// </summary>
public class PauseController : MonoBehaviour
{
    public GameObject menuPause;
    [SerializeField] GLPlayerController playerController;

    /// <summary>
    /// Comprueba si se ha presionado la tecla Escape para alternar la pausa.
    /// </summary>
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            print("esc was pressed!");
            TogglePause();
        }
    }

    /// <summary>
    /// Alterna entre el estado de juego pausado y en curso.
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
    /// Pausa el juego, activando el menú y deshabilitando el control del jugador.
    /// </summary>
    private void PauseGame()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        playerController.enabled = false;
    }

    /// <summary>
    /// Reanuda el juego, ocultando el menú y restaurando el control del jugador.
    /// </summary>
    private void ContinueGame()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        playerController.enabled = true;
    }
}