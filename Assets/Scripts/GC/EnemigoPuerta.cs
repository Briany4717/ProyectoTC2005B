using UnityEngine;
using System.Collections;

public class EnemigoPuerta : MonoBehaviour
{
    [Header("Posiciones de aparición")]
    public Transform posicionPuertaIzquierda;
    public Transform posicionPuertaDerecha;

    [Header("Sprites de cada enemigo en puerta")]
    public Sprite[] spritesEnemigos;

    private SpriteRenderer sr;
    private bool ocupado = false;
    private bool puertaCerradaAtiempo = false;

    public static EnemigoPuerta instancia;

    void Awake()
    {
        instancia = this;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void MostrarEnPuerta(Enemigo enemigo, int indexEnemigo)
    {
        if (ocupado) return;
        ocupado = true;
        puertaCerradaAtiempo = false;
        StartCoroutine(RutinaAparicion(enemigo, indexEnemigo));
    }

    IEnumerator RutinaAparicion(Enemigo enemigo, int indexEnemigo)
    {
        bool izquierda = Random.value > 0.5f;
        Transform posicion = izquierda ? posicionPuertaIzquierda : posicionPuertaDerecha;

        transform.position = posicion.position;

        if (indexEnemigo < spritesEnemigos.Length && spritesEnemigos[indexEnemigo] != null)
            sr.sprite = spritesEnemigos[indexEnemigo];

        sr.enabled = true;

        float tiempoEsperando = 0f;
        while (tiempoEsperando < 10f)
        {
            if (PuertaCorrespondienteCerrada(izquierda))
            {
                puertaCerradaAtiempo = true;
                break;
            }

            if (!MenuPausa.instancia.EstaEnPausa())
                tiempoEsperando += Time.unscaledDeltaTime;

            yield return null;
        }

        if (!puertaCerradaAtiempo)
        {
            sr.enabled = false;
            ocupado = false;
            Debug.Log("¡Derrota! El gato entró");

            SistemaFinJuego.instancia.MostrarDerrota();
            yield break;
        }

        float tiempoDespedida = 0f;
        while (tiempoDespedida < 2f)
        {
            if (!MenuPausa.instancia.EstaEnPausa())
                tiempoDespedida += Time.unscaledDeltaTime;
            yield return null;
        }

        SistemaMonedas.instancia?.RegistrarCerrarPuerta();
        sr.enabled = false;
        ocupado = false;
        SistemaEnemigos.instancia.EnemigoSeFueDePuerta(enemigo);
    }

    bool PuertaCorrespondienteCerrada(bool izquierda)
    {
        var botones = FindObjectsByType<BotonPuerta>(FindObjectsSortMode.None);
        foreach (var boton in botones)
        {
            bool esIzquierda = boton.transform.position.x < 0;
            if (esIzquierda == izquierda && boton.EstaAnimacionTerminada())
                return true;
        }
        return false;
    }

    public void OcultarInmediatamente()
    {
        StopAllCoroutines();
        sr.enabled = false;
        ocupado = false;
    }
}