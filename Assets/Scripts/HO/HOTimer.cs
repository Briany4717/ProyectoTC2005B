using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HOTimer : MonoBehaviour
{
    public static HOTimer Instance { get; private set; }

    public TextMeshProUGUI timertext;
    public float remainingtime;
    public string scene;

    void Awake()
    {
        // Patrón singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingtime > 0)
        {
            remainingtime -= Time.deltaTime;    
        } else if (remainingtime <= 0)
        {
            remainingtime =0;
            GameOver();
        }
        
        int minutes = Mathf.FloorToInt(remainingtime/60);
        int seconds = Mathf.FloorToInt(remainingtime%60);
        timertext.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddTime(float seconds)
    {
        remainingtime += seconds;
    }
    void GameOver()
    {
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.SaveCoins();
        }
        SceneManager.LoadScene(scene);
    }
}
