using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el flujo general del juego, el temporizador y los paneles de instrucciones.
/// </summary>
public class GLGameControl : MonoBehaviour
{
    static public GLGameControl Instance;

    public GameObject instructionPanel;

    GLTimer timer;

    /// <summary>
    /// Activa o desactiva la visibilidad del panel de instrucciones.
    /// </summary>
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

    /// <summary>
    /// Alterna el estado de activación de un objeto de estación específico.
    /// </summary>
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

    /// <summary>
    /// Inicializa la instancia Singleton y busca el temporizador en la escena.
    /// </summary>
    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        timer = FindAnyObjectByType<GLTimer>();
    }

    /// <summary>
    /// Carga la escena final cuando el juego termina.
    /// </summary>
    public void GameOver()
    {
        SceneManager.LoadScene("GLFinalScene");
    }

    /// <summary>
    /// Carga la escena del menú principal del juego.
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Resta una cantidad específica de segundos al temporizador global.
    /// </summary>
    public void RemoveTime(float seconds)
    {
        if (timer != null)
        {
            timer.remainingTime -= seconds;
        }
    }
}