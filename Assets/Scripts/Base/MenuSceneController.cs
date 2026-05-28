using UnityEngine;
using UnityEngine.SceneManagement;


/// Controlador para el manejo y navegación de escenas desde el menú principal.

public class MenuSceneController : MonoBehaviour
{
    
    /// Carga la escena inicial del juego de los Glotones.
    
    public void GoToGlotones()
    {
        SceneManager.LoadScene("GLStartScene");
    }

    
    /// Carga la escena de introducción del juego de HO.
    
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

    public void GoToGC()
    {
        SceneManager.LoadScene("GCMenuPrincipal");
    }

}
