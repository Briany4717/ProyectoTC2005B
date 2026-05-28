using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el tiempo restante de la partida y termina el juego al agotarse.
/// </summary>
public class HOTimer : MonoBehaviour
{
    public static HOTimer Instance { get; private set; }

    public TextMeshProUGUI timertext;
    public float remainingtime;

    /// <summary>
    /// Configura el singleton del temporizador.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Resta tiempo en cada frame y formatea el texto en pantalla.
    /// </summary>
    void Update()
    {
        if(remainingtime > 0)
        {
            remainingtime -= Time.deltaTime;    
        } else if (remainingtime <= 0)
        {
            remainingtime = 0;
            GameOver();
        }
        
        int minutes = Mathf.FloorToInt(remainingtime / 60);
        int seconds = Mathf.FloorToInt(remainingtime % 60);
        timertext.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Añade tiempo adicional al temporizador.
    /// </summary>
    public void AddTime(float seconds)
    {
        remainingtime += seconds;
    }
    
    /// <summary>
    /// Finaliza el juego guardando las monedas y cargando la escena final.
    /// </summary>
    void GameOver()
    {
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.SaveCoins();
        }
        SceneManager.LoadScene("HOEndScene");
    }
}