using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class HOTimer : MonoBehaviour
{
    public TextMeshProUGUI timertext;
    public float remainingtime;

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

    void GameOver()
    {
        SceneManager.LoadScene("HOEndScene");
    }
}
