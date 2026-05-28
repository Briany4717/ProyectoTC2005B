using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SistemaFinJuego : MonoBehaviour
{
    [Header("Pantalla Derrota")]
    public GameObject panelDerrota;
    public Button btnReiniciarDerrota;
    public Button btnMenuDerrota;

    [Header("Pantalla Victoria")]
    public GameObject panelVictoria;
    public Button btnReiniciarVictoria;
    public Button btnMenuVictoria;

    public static SistemaFinJuego instancia;

    void Awake() => instancia = this;

    void Start()
    {
        panelDerrota.SetActive(false);
        panelVictoria.SetActive(false);

        btnReiniciarDerrota.onClick.AddListener(Reiniciar);
        btnMenuDerrota.onClick.AddListener(IrAlMenu);
        btnReiniciarVictoria.onClick.AddListener(Reiniciar);
        btnMenuVictoria.onClick.AddListener(IrAlMenu);
    }

    public void MostrarDerrota()
    {
        // Detener el reloj
        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.SetPausa(true);

        MusicController.instancia?.DetenerMusica();
        Time.timeScale = 0f;
        SistemaMonedas.instancia?.MostrarResumen();
        panelDerrota.SetActive(true);
    }

    public void MostrarVictoria()
    {
        Time.timeScale = 0f;
        SistemaMonedas.instancia?.MostrarResumen();
        panelVictoria.SetActive(true);
    }

    void Reiniciar()
    {
        MusicController.instancia?.ReiniciarMusica();
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}