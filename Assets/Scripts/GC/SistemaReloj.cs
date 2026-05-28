using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


/// Controla el tiempo de juego, la progresión de las horas y el fin de la partida.

public class SistemaReloj : MonoBehaviour
{
    [Header("Reloj físico")]
    public SpriteRenderer spriteReloj;
    public Sprite[] spritesHoras;

    [Header("Hora digital")]
    public TMP_Text textoHoraDigital;
    public TMP_Text textoHoraDigitalPC;

    [Header("Secuencia de victoria")]
    public GameObject panelNegro;
    public TMP_Text textoHoraVictoria;
    public float tiempoParpadeoReloj = 3f;
    public float tiempoParpadeoPanel = 3f;

    [Header("Configuración")]
    public float segundosPorHora = 60f;

    private int horaActual = 0;
    private float temporizador = 0f;
    private bool juegoTerminado = false;
    private bool pausadoPorMenu = false;

    private string[] nombresHoras = { "12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM" };

    public static SistemaReloj instancia;

    
    /// Asigna la instancia principal del reloj.
    
    void Awake() => instancia = this;

    
    /// Inicializa la UI del reloj y los paneles.
    
    void Start()
    {
        if (panelNegro != null) panelNegro.SetActive(false);
        if (textoHoraVictoria != null) textoHoraVictoria.gameObject.SetActive(false);
        ActualizarReloj();
    }

    
    /// Actualiza el temporizador y avanza la hora si corresponde.
    
    void Update()
    {
        if (juegoTerminado) return;
        if (pausadoPorMenu) return;

        temporizador += Time.unscaledDeltaTime;

        if (temporizador >= segundosPorHora)
        {
            temporizador = 0f;
            AvanzarHora();
        }
    }

    
    /// Incrementa la hora actual y ajusta la dificultad.
    
    void AvanzarHora()
    {
        horaActual++;

        SistemaMonedas.instancia?.RegistrarHoraSobrevivida();

        if (horaActual >= nombresHoras.Length - 1)
        {
            horaActual = nombresHoras.Length - 1;
            ActualizarReloj();
            FinDelJuego();
            return;
        }

        ActualizarReloj();
        AumentarDificultad();
        Debug.Log($"Hora: {nombresHoras[horaActual]}");
    }

    
    /// Refleja la hora actual en los elementos visuales del reloj.
    
    void ActualizarReloj()
    {
        if (spriteReloj != null && horaActual < spritesHoras.Length)
            spriteReloj.sprite = spritesHoras[horaActual];

        if (textoHoraDigital != null)
            textoHoraDigital.text = nombresHoras[horaActual];

        if (textoHoraDigitalPC != null)
            textoHoraDigitalPC.text = nombresHoras[horaActual];
    }

    
    /// Aumenta la velocidad de movimiento de los enemigos según la hora.
    
    void AumentarDificultad()
    {
        float multiplicador = 1f - (horaActual * 0.15f);
        multiplicador = Mathf.Clamp(multiplicador, 0.3f, 1f);

        foreach (var enemigo in SistemaEnemigos.instancia.enemigos)
        {
            if (!enemigo.eliminado)
                enemigo.tiempoMovimiento = enemigo.tiempoMovimientoOriginal * multiplicador;
        }
    }

    
    /// Finaliza el progreso del juego externamente (victoria o derrota).
    
    public void TerminarJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        StopAllCoroutines();
        DetenerTodo();
    }

    
    /// Gestiona internamente el fin del juego al llegar a las 6 AM.
    
    void FinDelJuego()
    {
        juegoTerminado = true;
        SistemaMonedas.instancia?.RegistrarVictoria();
        DetenerTodo();
        StartCoroutine(SecuenciaVictoria());
    }

    
    /// Detiene y oculta a los enemigos y menús activos.
    
    void DetenerTodo()
    {
        foreach (var enemigo in SistemaEnemigos.instancia.enemigos)
            enemigo.eliminado = true;

        SistemaEnemigos.instancia.MostrarAdvertencia(false);

        if (SistemaCamaras.instancia != null)
            SistemaCamaras.instancia.CerrarCamaras();

        if (SistemaPreguntas.instancia != null)
            SistemaPreguntas.instancia.ForzarCierre();

        if (EnemigoPuerta.instancia != null)
            EnemigoPuerta.instancia.OcultarInmediatamente();
    }

    
    /// Reproduce la animación y sonidos de victoria al terminar la noche.
    
    IEnumerator SecuenciaVictoria()
    {
        MusicController.instancia?.PlayAlarm();

        yield return StartCoroutine(ParpadeaTexto(textoHoraDigital, tiempoParpadeoReloj));

        if (panelNegro != null)
        {
            panelNegro.SetActive(true);

            if (textoHoraVictoria != null)
            {
                textoHoraVictoria.gameObject.SetActive(true);
                textoHoraVictoria.text = "6:00 AM";
                yield return StartCoroutine(ParpadeaTexto(textoHoraVictoria, tiempoParpadeoPanel));
            }
        }

        MusicController.instancia?.StopAlarm();
        MusicController.instancia?.PlayWin();

        SistemaFinJuego.instancia.MostrarVictoria();
    }

    
    /// Hace parpadear un texto específico durante un tiempo determinado.
    
    IEnumerator ParpadeaTexto(TMP_Text texto, float duracion)
    {
        if (texto == null) yield break;

        float timer = 0f;
        while (timer < duracion)
        {
            texto.enabled = !texto.enabled;
            yield return new WaitForSecondsRealtime(0.2f);
            timer += 0.2f;
        }
        texto.enabled = true;
    }

    
    /// Pausa o reanuda el reloj.
    
    public void SetPausa(bool pausado) => pausadoPorMenu = pausado;

    
    /// Obtiene el string de la hora actual.
    
    public string GetHoraActual()      => nombresHoras[horaActual];

    
    /// Obtiene el índice numérico de la hora actual.
    
    public int    GetIndexHora()       => horaActual;
}