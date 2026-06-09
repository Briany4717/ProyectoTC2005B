using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

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
        PlayerPrefs.SetInt("Win",1);
        
        AddCoinsM datos = new AddCoinsM
        {
            id_usuario = PlayerPrefs.GetInt("id_usuario"),
            cantidad = PlayerPrefs.GetInt("Coins"),
            tipo_movimiento = 1
        };
        string jsonEnviar = JsonConvert.SerializeObject(datos);
        
        Debug.Log("JSON: " + jsonEnviar);

        ApiManager.Instance.Post(
            "agregarMonedas", 
            jsonEnviar, 
            onSuccess: (respuesta) => 
            {
                Debug.Log("Exito: " + respuesta);
            }, 
            onError: (error) => 
            {
                Debug.LogError("Error: " + error);
            }
        );
        SceneManager.LoadScene("PNFinalScene");
    }
}
