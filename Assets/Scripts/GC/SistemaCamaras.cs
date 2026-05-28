using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


/// Controla la interfaz de las cámaras y la computadora dentro del juego.

public class SistemaCamaras : MonoBehaviour
{
    [Header("UI")]
    public GameObject pantallaCamaras;
    public Image vistaCamara;
    public Transform contenedorEnemigos;
    public GameObject panelMapa;
    public GameObject panelVista;

    [Header("Menu Principal PC")]
    public GameObject panelMenuPC;
    public Button btnGenerador;
    public Button btnCamaras;

    [Header("Sprites de cada cámara")]
    public Sprite[] imagenesCamaras;

    [Header("Nombres de cámaras (opcional)")]
    public string[] nombresCamaras;

    [Header("Configuración")]
    public int camaraActual = 0;

    private bool abierto = false;
    private bool enVistaCamara = false;

    public static SistemaCamaras instancia;

    
    /// Configura el Singleton e inicializa ocultando las cámaras.
    
    void Awake()
    {
        instancia = this;
        pantallaCamaras.SetActive(false);
    }

    
    /// Asigna los eventos de los botones del menú de la PC.
    
    void Start()
    {
        btnGenerador.onClick.AddListener(() =>
        {
            panelMenuPC.SetActive(false);
            SistemaPreguntas.instancia.AbrirGenerador();
        });

        btnCamaras.onClick.AddListener(() =>
        {
            panelMenuPC.SetActive(false);
            MostrarCamaras();
        });
    }

    
    /// Permite cambiar entre cámaras usando las flechas del teclado.
    
    void Update()
    {
        if (!enVistaCamara) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            CamaraAnterior();

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            CamaraSiguiente();
    }

    
    /// Cambia a la siguiente cámara en orden.
    
    void CamaraSiguiente()
    {
        int siguiente = (camaraActual + 1) % imagenesCamaras.Length;
        CambiarCamara(siguiente);
    }

    
    /// Cambia a la cámara anterior en orden.
    
    void CamaraAnterior()
    {
        int anterior = (camaraActual - 1 + imagenesCamaras.Length) % imagenesCamaras.Length;
        CambiarCamara(anterior);
    }

    
    /// Muestra el menú principal de la PC y detiene el tiempo.
    
    public void AbrirMenuPC()
    {
        MusicController.instancia?.PlayCameras();
        abierto = true;
        enVistaCamara = false;
        pantallaCamaras.SetActive(true);
        panelMenuPC.SetActive(true);
        panelMapa.SetActive(false);
        panelVista.SetActive(false);
        Time.timeScale = 0f;
    }

    
    /// Oculta el menú de PC y muestra el mapa de cámaras.
    
    void MostrarCamaras()
    {
        abierto = true;
        panelMenuPC.SetActive(false);
        MostrarMapa();
    }

    
    /// Oculta la interfaz de PC/cámaras y reanuda el juego.
    
    public void CerrarCamaras()
    {
        abierto = false;
        enVistaCamara = false;
        pantallaCamaras.SetActive(false);
        panelMenuPC.SetActive(false);
        panelMapa.SetActive(false);
        panelVista.SetActive(false);
        Time.timeScale = 1f;
    }

    
    /// Activa el panel del mapa desactivando la vista de cámara actual.
    
    void MostrarMapa()
    {
        enVistaCamara = false;
        panelMapa.SetActive(true);
        panelVista.SetActive(false);
    }

    
    /// Muestra lo que hay en una cámara específica, incluyendo a los enemigos.
    
    public void CambiarCamara(int index)
    {
        camaraActual = index;
        enVistaCamara = true;
        vistaCamara.sprite = imagenesCamaras[index];

        panelMapa.SetActive(false);
        panelVista.SetActive(true);

        foreach (Transform hijo in contenedorEnemigos)
            hijo.gameObject.SetActive(false);

        SistemaEnemigos.instancia.MostrarEnemigosEnCamara(index);
    }

    
    /// Vuelve de la vista de una cámara al plano general del mapa.
    
    public void VolverAlMapa()
    {
        MostrarMapa();
    }

    
    /// Cierra todo para regresar al menú principal de la PC.
    
    public void VolverAlMenu()
    {
        enVistaCamara = false;
        panelMapa.SetActive(false);
        panelVista.SetActive(false);
        panelMenuPC.SetActive(true);
    }
}