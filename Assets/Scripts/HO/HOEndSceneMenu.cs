using UnityEngine;
using UnityEngine.SceneManagement;


/// Controla las opciones del menú al finalizar la partida.

public class HOEndSceneMenu : MonoBehaviour
{
    
    /// Restablece la escala de tiempo al iniciar la escena.
    
    void Start()
    {
        Time.timeScale = 1f;
    }

    
    /// Reinicia la escena principal del juego.
    
    public void RestartGame()
    {
        SceneManager.LoadScene("HOGameScene");
    }

    
    /// Carga la escena del menú de introducción.
    
    public void GoToMenu()
    {
        SceneManager.LoadScene("HOIntroScene");
    }
}