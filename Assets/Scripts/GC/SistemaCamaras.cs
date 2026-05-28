using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    void Awake()
    {
        instancia = this;
        pantallaCamaras.SetActive(false);
    }

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

    void Update()
    {
        // Carrusel solo funciona cuando estamos en vista de cámara
        if (!enVistaCamara) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            CamaraAnterior();

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            CamaraSiguiente();
    }

    void CamaraSiguiente()
    {
        int siguiente = (camaraActual + 1) % imagenesCamaras.Length;
        CambiarCamara(siguiente);
    }

    void CamaraAnterior()
    {
        int anterior = (camaraActual - 1 + imagenesCamaras.Length) % imagenesCamaras.Length;
        CambiarCamara(anterior);
    }

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

    void MostrarCamaras()
    {
        abierto = true;
        panelMenuPC.SetActive(false);
        MostrarMapa();
    }

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

    void MostrarMapa()
    {
        enVistaCamara = false;
        panelMapa.SetActive(true);
        panelVista.SetActive(false);
    }

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

    public void VolverAlMapa()
    {
        MostrarMapa();
    }

    public void VolverAlMenu()
    {
        enVistaCamara = false;
        panelMapa.SetActive(false);
        panelVista.SetActive(false);
        panelMenuPC.SetActive(true);
    }
}