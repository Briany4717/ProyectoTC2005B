using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class GLTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {

            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            GLGameControl.Instance.GameOver();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);


    }
}
