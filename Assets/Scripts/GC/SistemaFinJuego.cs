using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// Gestiona la transición a las pantallas de victoria y derrota.

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

    
    /// Establece la instancia Singleton del sistema.
    
    void Awake() => instancia = this;

    
    /// Inicializa las pantallas y asigna los eventos de los botones.
    
    void Start()
    {
        panelDerrota.SetActive(false);
        panelVictoria.SetActive(false);

        btnReiniciarDerrota.onClick.AddListener(Reiniciar);
        btnMenuDerrota.onClick.AddListener(IrAlMenu);
        btnReiniciarVictoria.onClick.AddListener(Reiniciar);
        btnMenuVictoria.onClick.AddListener(IrAlMenu);
    }

    
    /// Pausa el juego y muestra la pantalla de derrota.
    
    public void MostrarDerrota()
    {
        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.TerminarJuego();

        MusicController.instancia?.DetenerMusica();
        MusicController.instancia?.PlayDerrota();

        Time.timeScale = 0f;
        panelDerrota.SetActive(true);
    }

    
    /// Pausa el juego y muestra la pantalla de victoria con el resumen de monedas.
    
    public void MostrarVictoria()
    {
        Time.timeScale = 0f;
        SistemaMonedas.instancia?.MostrarResumen();
        panelVictoria.SetActive(true);
    }

    
    /// Reinicia la escena actual del juego.
    
    void Reiniciar()
    {
        MusicController.instancia?.ReiniciarMusica();
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    
    /// Carga la escena del menú principal.
    
    void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}