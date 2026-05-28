using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

/// <summary>
/// Administra el tiempo restante de la partida y finaliza el juego si llega a cero.
/// </summary>
public class GLTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float remainingTime;
    private bool gameOverTriggered = false;

    /// <summary>
    /// Reduce el tiempo restante cada frame y actualiza el texto en pantalla.
    /// </summary>
    void Update()
    {
        if (gameOverTriggered)
        {
            return;
        }

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            gameOverTriggered = true;
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