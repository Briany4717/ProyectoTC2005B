using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

public class PNGameController : MonoBehaviour
{
    public static PNGameController Instance; 
    public PNGUIController uiController;
    public PNSceneChanger sceneChanger;
    public GameObject pausePanel;
    private bool isPaused = false;
  
    public void Awake()
    {
        Instance = this;

        StopAllCoroutines();
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("HuntedPrompts", 0);
        PlayerPrefs.SetInt("id_usuario", 1);
        PlayerPrefs.SetInt("Win", 1); 
        SetReferences(); 
        sceneChanger = FindAnyObjectByType<PNSceneChanger>();
    }

    void SetReferences()
    {
        if(uiController == null)
            uiController = FindAnyObjectByType<PNGUIController>();
    }

    public void AddCoin()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 30);
        uiController.setCoin();
    }

    public void SpendCoins()
    {
        PlayerPrefs.SetInt("Coins", 0); 
        uiController.setCoin(); 
    }

    public void gameOver()
    {
        StopAllCoroutines();
        AddCoinsM datos = new AddCoinsM
        {
            id_usuario = PlayerPrefs.GetInt("id_usuario"),
            cantidad = PlayerPrefs.GetInt("Coins"),
            tipo_movimiento = 1
        };
        string jsonEnviar = JsonConvert.SerializeObject(datos);
        
        Debug.Log("JSON: " + jsonEnviar);

        ApiManager.Instance.Post(
            "agregarMonedas", 
            jsonEnviar, 
            onSuccess: (respuesta) => 
            {
                Debug.Log("Exito: " + respuesta);
            }, 
            onError: (error) => 
            {
                Debug.LogError("Error: " + error);
            }
        );
        sceneChanger.change("PNFinalScene");
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void ExitGame()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt("Win",1);
        
        AddCoinsM datos = new AddCoinsM
        {
            id_usuario = PlayerPrefs.GetInt("id_usuario"),
            cantidad = PlayerPrefs.GetInt("Coins"),
            tipo_movimiento = 1
        };
        string jsonEnviar = JsonConvert.SerializeObject(datos);
        
        Debug.Log("JSON: " + jsonEnviar);

        ApiManager.Instance.Post(
            "agregarMonedas", 
            jsonEnviar, 
            onSuccess: (respuesta) => 
            {
                Debug.Log("Exito: " + respuesta);
            }, 
            onError: (error) => 
            {
                Debug.LogError("Error: " + error);
            }
        );
        sceneChanger.change("PNFinalScene");
    }

    void Start()
    {
    }
}