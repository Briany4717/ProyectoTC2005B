using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;


/// Controla la pantalla final mostrando estadísticas y el puntaje obtenido en forma de estrellas.

public class GLFinalSceneControler : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI ordersText;

    public Image star1;
    public Image star2;
    public Image star3;

    
    /// Carga los puntajes guardados y calcula la cantidad de estrellas a mostrar.
    
    void Start()
    {
        int orders = PlayerPrefs.GetInt("Orders");
        int coins = PlayerPrefs.GetInt("Coins");
        if (coins == 1)
            coinsText.text = coins.ToString() + " moneda!";
        else
            coinsText.text = coins.ToString() + " monedas!";
        if (orders == 1)
            ordersText.text = orders.ToString() + " orden!";
        else
            ordersText.text = orders.ToString() + " ordenes!";


        if (orders >= 6)
        {
        }
        else if (orders >= 4)
        {
            star3.gameObject.SetActive(false);
        }
        else if (orders >= 2)
        {
            star2.gameObject.SetActive(false);
            star3.gameObject.SetActive(false);
        }
        else
        {
            star1.gameObject.SetActive(false);
            star2.gameObject.SetActive(false);
            star3.gameObject.SetActive(false);
        }
    }

    
    /// Reinicia la escena principal del minijuego.
    
    public void RestartGame()
    {
        SceneManager.LoadScene("GLMainGame");
    }

    
    /// Carga la escena principal del menú del juego general.
    
    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}