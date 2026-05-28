using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PNtimer : MonoBehaviour
{

    public TextMeshProUGUI timertext;
    public float remainingtime;
    public string scene;

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
        StopAllCoroutines();
        PlayerPrefs.SetInt("Win",0);
        SceneManager.LoadScene("PNFinalScene");
    }
}
