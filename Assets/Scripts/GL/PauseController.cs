using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public GameObject menuPause;
    public static bool isPaused { get; private set; } = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            print("esc was pressed!");
            TogglePause();
        }
    }

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
    private void PauseGame()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
    private void ContinueGame()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

}
