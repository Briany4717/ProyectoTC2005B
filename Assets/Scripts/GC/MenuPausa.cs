using UnityEngine;
using UnityEngine.SceneManagement;


/// Controla la pausa del juego, tanto en la vista principal como en las cámaras.

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

    
    /// Configura el Singleton al iniciar.
    
    void Awake() => instancia = this;

    
    /// Oculta todos los menús de pausa al arrancar la escena.
    
    void Start()  => OcultarTodosLosPaneles();

    
    /// Escucha la tecla de escape para alternar entre pausa y reanudar.
    
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausado) Reanudar();
            else         Pausar();
        }
    }

    
    /// Detiene el tiempo y despliega el panel de pausa correspondiente a la vista.
    
    public void Pausar()
    {
        pausado = true;
        pausadoEnCamaras = EnCamaras;
        botonUsado = EnCamaras ? botonPausaUICamaras : botonPausaUIJuego;
        SetPanel(PanelPausaSegun(pausadoEnCamaras), true);
        Time.timeScale = 0f;

        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.SetPausa(true);
    }

    
    /// Reanuda el flujo normal del tiempo y oculta los paneles de pausa.
    
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

        if (SistemaReloj.instancia != null)
            SistemaReloj.instancia.SetPausa(false);
    }

    
    /// Oculta el menú de pausa y muestra la configuración.
    
    public void AbrirConfiguracion()
    {
        SetPanel(PanelPausaSegun(pausadoEnCamaras),  false);
        SetPanel(PanelConfigSegun(pausadoEnCamaras), true);
    }

    
    /// Oculta el menú de pausa y muestra las instrucciones.
    
    public void AbrirInstrucciones()
    {
        SetPanel(PanelPausaSegun(pausadoEnCamaras), false);
        SetPanel(PanelInstrSegun(pausadoEnCamaras), true);
    }

    
    /// Regresa a la vista principal del menú de pausa.
    
    public void VolverAPausa()
    {
        SetPanel(PanelConfigSegun(pausadoEnCamaras), false);
        SetPanel(PanelInstrSegun(pausadoEnCamaras),  false);
        SetPanel(PanelPausaSegun(pausadoEnCamaras),  true);
    }

    
    /// Vuelve a cargar la escena de juego.
    
    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    
    /// Carga la escena del menú principal.
    
    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    
    /// Determina qué panel de pausa usar según el contexto.
    
    private GameObject PanelPausaSegun(bool enCamaras) =>
        enCamaras ? panelPausa         : panelPausaJuego;

    
    /// Determina qué panel de configuración usar según el contexto.
    
    private GameObject PanelConfigSegun(bool enCamaras) =>
        enCamaras ? panelConfiguracion : panelConfiguracionJuego;

    
    /// Determina qué panel de instrucciones usar según el contexto.
    
    private GameObject PanelInstrSegun(bool enCamaras) =>
        enCamaras ? panelInstrucciones : panelInstruccionesJuego;

    
    /// Cambia el estado activo de un panel específico.
    
    private void SetPanel(GameObject panel, bool estado)
    {
        if (panel != null) panel.SetActive(estado);
    }

    
    /// Apaga todos los paneles referenciados por este menú.
    
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