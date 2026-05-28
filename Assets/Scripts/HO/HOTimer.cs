using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


/// Gestiona el tiempo restante de la partida y termina el juego al agotarse.

public class HOTimer : MonoBehaviour
{
    public static HOTimer Instance { get; private set; }

    public TextMeshProUGUI timertext;
    public float remainingtime;
    public string scene;

    
    /// Configura el singleton del temporizador.
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// Resta tiempo en cada frame y formatea el texto en pantalla.
    
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

    
    /// Añade tiempo adicional al temporizador.
    
    public void AddTime(float seconds)
    {
        remainingtime += seconds;
    }
    
    
    /// Finaliza el juego guardando las monedas y cargando la escena final.
    
    void GameOver()
    {
        if (HOCoins.Instance != null)
        {
            HOCoins.Instance.SaveCoins();
        }
        SceneManager.LoadScene(scene);
    }
}