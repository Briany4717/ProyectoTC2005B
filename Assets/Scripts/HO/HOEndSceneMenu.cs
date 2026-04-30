using UnityEngine;
using UnityEngine.SceneManagement;

public class HOEndSceneMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("HOGameScene");

    }

    public void GoToMenu()
    {

        SceneManager.LoadScene("HOIntroScene");
    }
}
