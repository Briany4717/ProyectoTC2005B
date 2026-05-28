using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour
{


    public void GoToGlotones()
    {
        SceneManager.LoadScene("GLStartScene");
    }

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
