using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;


/// Controla el flujo general del juego, el temporizador y los paneles de instrucciones.

public class GLGameControl : MonoBehaviour
{
    static public GLGameControl Instance;

    public GameObject instructionPanel;

    GLTimer timer;

    
    /// Activa o desactiva la visibilidad del panel de instrucciones.
    
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

    
    /// Alterna el estado de activación de un objeto de estación específico.
    
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

    
    /// Inicializa la instancia Singleton y busca el temporizador en la escena.
    
    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        timer = FindAnyObjectByType<GLTimer>();
    }

    
    /// Carga la escena final cuando el juego termina.
    
    public void GameOver()
    {
        SceneManager.LoadScene("GLFinalScene");
    }

    
    /// Carga la escena del menú principal del juego.
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    
    /// Resta una cantidad específica de segundos al temporizador global.
    
    public void RemoveTime(float seconds)
    {
        if (timer != null)
        {
            timer.remainingTime -= seconds;
        }
    }
}