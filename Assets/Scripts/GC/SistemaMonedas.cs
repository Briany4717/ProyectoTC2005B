using UnityEngine;
using TMPro;


/// Administra la economía del juego y las recompensas por acciones del jugador.

public class SistemaMonedas : MonoBehaviour
{
    [Header("Monedas por acción")]
    public int monedasPorEliminarEnemigo  = 10;
    public int monedasPorCerrarPuerta     = 5;
    public int monedasPorVictoria         = 50;
    public int monedasPorPreguntaCorrecta = 3;
    public int monedasPorHoraSobrevivida  = 15;

    [Header("UI — texto de monedas en pantalla de victoria/derrota")]
    public TMP_Text textoMonedasSesion;

    private int monedasSesion       = 0;
    private int enemigoEliminados   = 0;
    private int puertasCerradas     = 0;
    private int preguntasCorrectas  = 0;
    private int horasSobrevividas   = 0;
    private bool victoriaRegistrada = false;

    public static SistemaMonedas instancia;

    
    /// Configura el Singleton.
    
    void Awake() => instancia = this;

    
    /// Añade monedas por eliminar a un enemigo con electricidad.
    
    public void RegistrarEliminarEnemigo()
    {
        enemigoEliminados++;
        AgregarMonedas(monedasPorEliminarEnemigo);
    }

    
    /// Añade monedas por haber cerrado la puerta a un enemigo a tiempo.
    
    public void RegistrarCerrarPuerta()
    {
        puertasCerradas++;
        AgregarMonedas(monedasPorCerrarPuerta);
    }

    
    /// Añade monedas tras responder correctamente en el generador.
    
    public void RegistrarPreguntaCorrecta()
    {
        preguntasCorrectas++;
        AgregarMonedas(monedasPorPreguntaCorrecta);
    }

    
    /// Añade monedas cuando pasa una hora en el juego.
    
    public void RegistrarHoraSobrevivida()
    {
        horasSobrevividas++;
        AgregarMonedas(monedasPorHoraSobrevivida);
    }

    
    /// Añade monedas de bonificación por ganar la partida.
    
    public void RegistrarVictoria()
    {
        if (victoriaRegistrada) return;
        victoriaRegistrada = true;
        AgregarMonedas(monedasPorVictoria);
    }

    
    /// Suma a la cantidad temporal de la sesión.
    
    void AgregarMonedas(int cantidad)
    {
        monedasSesion += cantidad;
    }

    
    /// Guarda las monedas de la sesión en el total acumulado y actualiza la UI.
    
    public void MostrarResumen()
    {
        int totalGuardado = PlayerPrefs.GetInt("MonedasTotal", 0);
        int nuevoTotal    = totalGuardado + monedasSesion;
        PlayerPrefs.SetInt("MonedasTotal", nuevoTotal);
        PlayerPrefs.Save();

        if (textoMonedasSesion != null)
            textoMonedasSesion.text = $"{monedasSesion}";
    }

    
    /// Devuelve la cantidad total de monedas guardadas.
    
    public int GetMonedasTotal() => PlayerPrefs.GetInt("MonedasTotal", 0);
}