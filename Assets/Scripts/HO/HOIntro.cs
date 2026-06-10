using UnityEngine;
using UnityEngine.SceneManagement;

public class HOIntro : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene("HOGameScene");
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

}
