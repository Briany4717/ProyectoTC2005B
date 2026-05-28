using UnityEngine;
using UnityEngine.SceneManagement;


/// Controla la navegación en la pantalla de inicio del juego.

public class GLPantallaInicial : MonoBehaviour
{
    
    /// Carga la escena principal del juego para comenzar a jugar.
    
    public void StartGame()
    {
        SceneManager.LoadScene("GLMainGame");
    }

    
    /// Carga la escena del menú general del proyecto.
    
    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}