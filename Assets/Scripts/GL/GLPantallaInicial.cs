using UnityEngine;
using UnityEngine.SceneManagement;

public class GLPantallaInicial : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GLMainGame");

    }

    public void GoToMenu()
    {

        SceneManager.LoadScene("MenuScene");
    }

}
