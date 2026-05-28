using System.Collections;
using UnityEngine;

public class PNGameController : MonoBehaviour
{
    public static PNGameController Instance; 
    public PNGUIController uiController;
    public PNSceneChanger sceneChanger;
    public GameObject pausePanel;
    private bool isPaused = false;
  
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StopAllCoroutines();
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("HuntedPrompts", 0);
        PlayerPrefs.SetInt("id_usuario", 1);
        PlayerPrefs.SetInt("Win", 0); 
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
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
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
        sceneChanger.change("PNFinalScene");
    }

    void Start()
    {
    }
}