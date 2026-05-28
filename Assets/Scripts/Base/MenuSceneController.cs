using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador para el manejo y navegación de escenas desde el menú principal.
/// </summary>
public class MenuSceneController : MonoBehaviour
{
    /// <summary>
    /// Carga la escena inicial del juego de los Glotones.
    /// </summary>
    public void GoToGlotones()
    {
        SceneManager.LoadScene("GLStartScene");
    }

    /// <summary>
    /// Carga la escena de introducción del juego de HO.
    /// </summary>
    public void GoToHO()
    {
        SceneManager.LoadScene("HOIntroScene");
    }
    public void GoToPN()
    {
        SceneManager.LoadScene("PNStartScene");
    }
    
    public void GoToLE()
    {
        SceneManager.LoadScene("LEConveyorScene");
    }
}
