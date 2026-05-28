using UnityEngine;
using TMPro;

public class SistemaMonedas : MonoBehaviour
{
    [Header("Monedas por acción")]
    public int monedasPorEliminarEnemigo  = 10;
    public int monedasPorCerrarPuerta     = 5;
    public int monedasPorVictoria         = 50;
    public int monedasPorPreguntaCorrecta = 3;
    public int monedasPorHoraSobrevivida  = 15;

    [Header("UI — texto de monedas en pantalla de victoria/derrota")]
    public TMP_Text textoMonedasSesion; // El TextoMonedas dentro del panel Monedas

    private int monedasSesion       = 0;
    private int enemigoEliminados   = 0;
    private int puertasCerradas     = 0;
    private int preguntasCorrectas  = 0;
    private int horasSobrevividas   = 0;
    private bool victoriaRegistrada = false;

    public static SistemaMonedas instancia;

    void Awake() => instancia = this;

    // --- Registrar acciones ---

    public void RegistrarEliminarEnemigo()
    {
        enemigoEliminados++;
        AgregarMonedas(monedasPorEliminarEnemigo);
    }

    public void RegistrarCerrarPuerta()
    {
        puertasCerradas++;
        AgregarMonedas(monedasPorCerrarPuerta);
    }

    public void RegistrarPreguntaCorrecta()
    {
        preguntasCorrectas++;
        AgregarMonedas(monedasPorPreguntaCorrecta);
    }

    public void RegistrarHoraSobrevivida()
    {
        horasSobrevividas++;
        AgregarMonedas(monedasPorHoraSobrevivida);
    }

    public void RegistrarVictoria()
    {
        if (victoriaRegistrada) return;
        victoriaRegistrada = true;
        AgregarMonedas(monedasPorVictoria);
    }

    void AgregarMonedas(int cantidad)
    {
        monedasSesion += cantidad;
    }

    public void MostrarResumen()
    {
        // Guardar en PlayerPrefs
        int totalGuardado = PlayerPrefs.GetInt("MonedasTotal", 0);
        int nuevoTotal    = totalGuardado + monedasSesion;
        PlayerPrefs.SetInt("MonedasTotal", nuevoTotal);
        PlayerPrefs.Save();

        // Mostrar monedas de esta sesión en el panel de victoria/derrota
        if (textoMonedasSesion != null)
            textoMonedasSesion.text = $"{monedasSesion}";
    }

    public int GetMonedasTotal() => PlayerPrefs.GetInt("MonedasTotal", 0);
}