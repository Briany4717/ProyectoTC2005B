using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles — Vista Cámaras")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion;
    public GameObject panelInstrucciones;
    public bool EstaEnPausa() => pausado;

    [Header("Paneles — Vista Juego Principal")]
    public GameObject panelPausaJuego;
    public GameObject panelConfiguracionJuego;
    public GameObject panelInstruccionesJuego;

    [Header("Contexto")]
    public GameObject pantallaCamaras;

    [Header("Nombres de escenas")]
    public string nombreEscenaJuego = "GCInicial";
    public string nombreEscenaMenu  = "GCMenuPrincipal";

    [Header("Botones pausa")]
    public BotonPausaUI botonPausaUICamaras;
    public BotonPausaUI botonPausaUIJuego;

    private bool pausado          = false;
    private bool pausadoEnCamaras = false;
    private BotonPausaUI botonUsado;

    public static MenuPausa instancia;

    private bool EnCamaras =>
        pantallaCamaras != null && pantallaCamaras.activeSelf;

    void Awake() => instancia = this;
    void Start()  => OcultarTodosLosPaneles();

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausado) Reanudar();
            else         Pausar();
        }
    }

    public void Pausar()
    {
        pausado = true;
        pausadoEnCamaras = EnCamaras;
        botonUsado = EnCamaras ? botonPausaUICamaras : botonPausaUIJuego;
        SetPanel(PanelPausaSegun(pausadoEnCamaras), true);
        Time.timeScale = 0f;

        // Detener el reloj
        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.SetPausa(true);
    }

    public void Reanudar()
    {
        pausado = false;
        SetPanel(PanelPausaSegun(pausadoEnCamaras), false);
        SetPanel(PanelConfigSegun(pausadoEnCamaras), false);
        SetPanel(PanelInstrSegun(pausadoEnCamaras), false);
        Time.timeScale = 1f;

        if (botonUsado != null)
            botonUsado.ResetearSprite();
        botonUsado = null;

        // Reanudar el reloj
        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.SetPausa(false);
    }

    public void AbrirConfiguracion()
    {
        SetPanel(PanelPausaSegun(pausadoEnCamaras),  false);
        SetPanel(PanelConfigSegun(pausadoEnCamaras), true);
    }

    public void AbrirInstrucciones()
    {
        SetPanel(PanelPausaSegun(pausadoEnCamaras), false);
        SetPanel(PanelInstrSegun(pausadoEnCamaras), true);
    }

    public void VolverAPausa()
    {
        SetPanel(PanelConfigSegun(pausadoEnCamaras), false);
        SetPanel(PanelInstrSegun(pausadoEnCamaras),  false);
        SetPanel(PanelPausaSegun(pausadoEnCamaras),  true);
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    private GameObject PanelPausaSegun(bool enCamaras) =>
        enCamaras ? panelPausa         : panelPausaJuego;
    private GameObject PanelConfigSegun(bool enCamaras) =>
        enCamaras ? panelConfiguracion : panelConfiguracionJuego;
    private GameObject PanelInstrSegun(bool enCamaras) =>
        enCamaras ? panelInstrucciones : panelInstruccionesJuego;

    private void SetPanel(GameObject panel, bool estado)
    {
        if (panel != null) panel.SetActive(estado);
    }

    private void OcultarTodosLosPaneles()
    {
        SetPanel(panelPausa,              false);
        SetPanel(panelConfiguracion,      false);
        SetPanel(panelInstrucciones,      false);
        SetPanel(panelPausaJuego,         false);
        SetPanel(panelConfiguracionJuego, false);
        SetPanel(panelInstruccionesJuego, false);
    }
}