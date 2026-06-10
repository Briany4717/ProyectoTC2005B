using UnityEngine;

public class HOGameManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
