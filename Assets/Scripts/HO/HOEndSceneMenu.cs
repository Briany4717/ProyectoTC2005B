using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla las opciones del menú al finalizar la partida.
/// </summary>
public class HOEndSceneMenu : MonoBehaviour
{
    /// <summary>
    /// Restablece la escala de tiempo al iniciar la escena.
    /// </summary>
    void Start()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Reinicia la escena principal del juego.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene("HOGameScene");
    }

    /// <summary>
    /// Carga la escena del menú de introducción.
    /// </summary>
    public void GoToMenu()
    {
        SceneManager.LoadScene("HOIntroScene");
    }
}