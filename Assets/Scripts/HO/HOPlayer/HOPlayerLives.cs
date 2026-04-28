using UnityEngine;
using UnityEngine.SceneManagement;

public class HOPlayerLives : MonoBehaviour
{
    public int cantidadDeVida;

    public void TomarDanio(int danio)
    {
        cantidadDeVida -= danio;
        if (cantidadDeVida <= 0)
        {
            SceneManager.LoadScene("HOEndScene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
