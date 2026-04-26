using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GLGameControl : MonoBehaviour
{
    static public GLGameControl Instance;



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
    }

    public void GameOver()
    {
        // se tendria que pasar los puntos o algo para que se muestren en final scene.
        SceneManager.LoadScene("GLFinalScene");

    }
}
