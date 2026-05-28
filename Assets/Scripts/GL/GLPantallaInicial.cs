using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la navegación en la pantalla de inicio del juego.
/// </summary>
public class GLPantallaInicial : MonoBehaviour
{
    /// <summary>
    /// Carga la escena principal del juego para comenzar a jugar.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("GLMainGame");
    }

    /// <summary>
    /// Carga la escena del menú general del proyecto.
    /// </summary>
    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}