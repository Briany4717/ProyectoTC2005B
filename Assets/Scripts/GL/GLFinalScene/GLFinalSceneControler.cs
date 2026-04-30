using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;

public class GLFinalSceneControler : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI ordersText;

    public Image star1;
    public Image star2;
    public Image star3;

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


        if (orders >= 10)
        {
            // dejar las estrellas activas
        }
        else if (orders >= 8)
        {
            star3.gameObject.SetActive(false);
        }
        else if (orders >= 3)
        {
            // desactivar las estrellas
            star2.gameObject.SetActive(false);
            star3.gameObject.SetActive(false);
        }
        else
        {
            // desactivar las estrellas
            star1.gameObject.SetActive(false);
            star2.gameObject.SetActive(false);
            star3.gameObject.SetActive(false);
        }

    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GLMainGame");

    }

    public void GoToMenu()
    {

        SceneManager.LoadScene("MenuScene");
    }
}
