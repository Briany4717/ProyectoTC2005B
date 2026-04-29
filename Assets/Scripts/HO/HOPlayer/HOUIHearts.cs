using UnityEngine;
using UnityEngine.UI;

public class HOUIHearts : MonoBehaviour
{
    public Sprite corazonLleno;
    public Sprite corazonMitad;
    public Sprite corazonVacio;

    public Image[] corazones;

    public HOPlayerLives playerLives;

    void Start()
    {
        playerLives.OnLivesChanged += UpdateHearts;

        UpdateHearts(playerLives.cantidadDeVida);
    }

    void OnDestroy()
    {
        if (playerLives != null)
        {
            playerLives.OnLivesChanged -= UpdateHearts;
        }
    }

    void UpdateHearts(int vidasRestantes)
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            int vidasParaCorazonLleno = (i + 1) * 2;
            int vidasParaCorazonMitad = (i * 2) + 1;

            if (vidasRestantes >= vidasParaCorazonLleno)
            {
                corazones[i].sprite = corazonLleno;
            }
            else if (vidasRestantes == vidasParaCorazonMitad)
            {
                corazones[i].sprite = corazonMitad;
            }
            else
            {
                corazones[i].sprite = corazonVacio;
            }
        }
    }
}