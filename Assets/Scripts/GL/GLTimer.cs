using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class GLTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float remainingTime;


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
            return;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }
}
