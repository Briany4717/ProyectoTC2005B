using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Actualiza la representación visual de la vida del jugador usando corazones.
/// </summary>
public class HOUIHearts : MonoBehaviour
{
    public Sprite corazonLleno;
    public Sprite corazonMitad;
    public Sprite corazonVacio;

    public Image[] corazones;

    public HOPlayerLives playerLives;

    /// <summary>
    /// Se suscribe al evento de cambio de vida y realiza la primera actualización.
    /// </summary>
    void Start()
    {
        playerLives.OnLivesChanged += UpdateHearts;
        UpdateHearts(playerLives.cantidadDeVida);
    }

    /// <summary>
    /// Se desuscribe del evento al destruirse.
    /// </summary>
    void OnDestroy()
    {
        if (playerLives != null)
        {
            playerLives.OnLivesChanged -= UpdateHearts;
        }
    }

    /// <summary>
    /// Actualiza los sprites de los corazones en base a la vida restante.
    /// </summary>
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