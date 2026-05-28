using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    void Awake() => instancia = this;

    void Start()
    {
        if (panelNegro != null) panelNegro.SetActive(false);
        if (textoHoraVictoria != null) textoHoraVictoria.gameObject.SetActive(false);
        ActualizarReloj();
    }

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

    void ActualizarReloj()
    {
        if (spriteReloj != null && horaActual < spritesHoras.Length)
            spriteReloj.sprite = spritesHoras[horaActual];

        if (textoHoraDigital != null)
            textoHoraDigital.text = nombresHoras[horaActual];

        if (textoHoraDigitalPC != null)
            textoHoraDigitalPC.text = nombresHoras[horaActual];
    }

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

    // ── Terminar juego (victoria o derrota) ───────────────────────────

    /// <summary>
    /// Detiene el reloj y todos los procesos.
    /// Llamado tanto por victoria (internamente) como por derrota (SistemaFinJuego).
    /// </summary>
    public void TerminarJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        StopAllCoroutines(); // Cancela SecuenciaVictoria si estaba corriendo
        DetenerTodo();
    }

    void FinDelJuego()
    {
        juegoTerminado = true;
        SistemaMonedas.instancia?.RegistrarVictoria();
        DetenerTodo();
        StartCoroutine(SecuenciaVictoria());
    }

    void DetenerTodo()
    {
        // Detener enemigos
        foreach (var enemigo in SistemaEnemigos.instancia.enemigos)
            enemigo.eliminado = true;

        // Ocultar advertencias
        SistemaEnemigos.instancia.MostrarAdvertencia(false);

        // Cerrar pantalla de cámaras si está abierta
        if (SistemaCamaras.instancia != null)
            SistemaCamaras.instancia.CerrarCamaras();

        // Cerrar panel del generador si está abierto
        if (SistemaPreguntas.instancia != null)
            SistemaPreguntas.instancia.ForzarCierre();

        // Detener enemigo en puerta si hay uno
        if (EnemigoPuerta.instancia != null)
            EnemigoPuerta.instancia.OcultarInmediatamente();
    }

    // ── Secuencia de victoria ─────────────────────────────────────────

    IEnumerator SecuenciaVictoria()
    {
        MusicController.instancia?.PlayAlarm();

        // Paso 1 — parpadear hora en HUD
        yield return StartCoroutine(ParpadeaTexto(textoHoraDigital, tiempoParpadeoReloj));

        // Paso 2 — mostrar panel negro con hora parpadeando
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

        // Paso 3 — mostrar pantalla de victoria
        SistemaFinJuego.instancia.MostrarVictoria();
    }

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

    // ── Utilidades públicas ───────────────────────────────────────────

    public void SetPausa(bool pausado) => pausadoPorMenu = pausado;
    public string GetHoraActual()      => nombresHoras[horaActual];
    public int    GetIndexHora()       => horaActual;

    
}