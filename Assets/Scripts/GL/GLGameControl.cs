using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GLGameControl : MonoBehaviour
{
    static public GLGameControl Instance;

    public GameObject instructionPanel;

    GLTimer timer;


    public void ToggleInstructionPanel()
    {
        if (instructionPanel.activeInHierarchy == false)
        {
            instructionPanel.SetActive(true);
        }
        else
        {
            instructionPanel.SetActive(false);
        }
    }

    public void TriggerMenu(GameObject stationObject)
    {
        if (stationObject.activeInHierarchy == false)
        {
            stationObject.SetActive(true);
        }
        else
        {
            stationObject.SetActive(false);
        }
    }

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        timer = FindAnyObjectByType<GLTimer>();
    }

    public void GameOver()
    {
        // se tendria que pasar los puntos o algo para que se muestren en final scene.
        SceneManager.LoadScene("GLFinalScene");

    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }


    // function to remove seconds from the timer, can be called from other scripts when certain events happen
    public void RemoveTime(float seconds)
    {
        if (timer != null)
        {
            timer.remainingTime -= seconds;
        }
    }

}
