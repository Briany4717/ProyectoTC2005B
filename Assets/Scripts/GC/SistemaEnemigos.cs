using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SistemaEnemigos : MonoBehaviour
{
    [Header("Enemigos en juego")]
    public List<Enemigo> enemigos;
    public Transform contenedorEnemigos;

    [Header("Puerta")]
    public GameObject advertenciaPuerta;
    public GameObject advertenciaPuertaPC;
    public float tiempoEsperaPuerta = 2f;

    private Enemigo enemigoEnPuerta = null;
    private bool puertaOcupada = false;

    public static SistemaEnemigos instancia;

    void Awake() => instancia = this;

    void Start()
    {
        if (advertenciaPuerta != null)
            advertenciaPuerta.SetActive(false);
        if (advertenciaPuertaPC != null)
            advertenciaPuertaPC.SetActive(false);

        foreach (var enemigo in enemigos)
        {
            enemigo.tiempoMovimientoOriginal = enemigo.tiempoMovimiento;
            StartCoroutine(MoverEnemigo(enemigo));
        }
    }

    IEnumerator MoverEnemigo(Enemigo enemigo)
    {
        while (true)
        {
            // Contador manual que respeta pausa de menú pero NO las cámaras
            float timer = 0f;
            while (timer < enemigo.tiempoMovimiento)
            {
                // Solo contar si NO hay pausa de menú
                if (!MenuPausa.instancia.EstaEnPausa())
                    timer += Time.unscaledDeltaTime;
                yield return null;
            }

            if (enemigo.eliminado) continue;

            if (puertaOcupada)
                yield return new WaitUntil(() => !puertaOcupada);

            enemigo.camaraActual--;

            if (enemigo.camaraActual < 0)
            {
                StartCoroutine(EnemigoEnPuerta(enemigo));
                yield break;
            }

            if (SistemaCamaras.instancia.camaraActual == enemigo.camaraActual)
                MostrarEnemigosEnCamara(enemigo.camaraActual);
        }
    }

    IEnumerator EnemigoEnPuerta(Enemigo enemigo)
    {
        puertaOcupada = true;
        enemigoEnPuerta = enemigo;

        MostrarAdvertencia(true);
        Debug.Log($"{enemigo.nombre} está en la puerta!");

        int indexEnemigo = enemigos.IndexOf(enemigo);
        EnemigoPuerta.instancia.MostrarEnPuerta(enemigo, indexEnemigo);

        yield return new WaitUntil(() => !puertaOcupada);

        StartCoroutine(MoverEnemigo(enemigo));
    }

    public void EnemigoSeFueDePuerta(Enemigo enemigo)
    {
        MostrarAdvertencia(false);
        enemigoEnPuerta = null;
        puertaOcupada = false;

        enemigo.camaraActual = 4;
        enemigo.eliminado = false;

        Debug.Log($"{enemigo.nombre} se fue de la puerta");
    }

    bool PuertaCerrada()
    {
        var botones = FindObjectsByType<BotonPuerta>(FindObjectsSortMode.None);
        foreach (var boton in botones)
        {
            if (boton.EstaAnimacionTerminada())
                return true;
        }
        return false;
    }

    public void MostrarEnemigosEnCamara(int indexCamara)
    {
        foreach (Transform hijo in contenedorEnemigos)
            Destroy(hijo.gameObject);

        foreach (var enemigo in enemigos)
        {
            if (!enemigo.eliminado && enemigo.camaraActual == indexCamara)
            {
                GameObject obj = Instantiate(enemigo.prefabUI, contenedorEnemigos);
                RectTransform rt = obj.GetComponent<RectTransform>();

                if (enemigo.posicionesPorCamara != null &&
                    indexCamara < enemigo.posicionesPorCamara.Length)
                    rt.anchoredPosition = enemigo.posicionesPorCamara[indexCamara];

                if (enemigo.escalasPorCamara != null &&
                    indexCamara < enemigo.escalasPorCamara.Length)
                {
                    float escala = enemigo.escalasPorCamara[indexCamara];
                    rt.localScale = new Vector3(escala, escala, 1f);
                }

                obj.GetComponent<EnemigoUI>().Inicializar(enemigo);
            }
        }
    }

    public void EliminarEnemigo(Enemigo enemigo)
    {
        enemigo.eliminado = true;
        StartCoroutine(RegresarAEntrada(enemigo));
    }

    IEnumerator RegresarAEntrada(Enemigo enemigo)
    {
        float timer = 0f;
        while (timer < 3f)
        {
            if (!MenuPausa.instancia.EstaEnPausa())
                timer += Time.unscaledDeltaTime;
            yield return null;
        }

        enemigo.camaraActual = 4;
        enemigo.eliminado = false;

        if (SistemaCamaras.instancia.camaraActual == 4)
            MostrarEnemigosEnCamara(4);
    }

    public void MostrarAdvertencia(bool mostrar)
    {
        if (advertenciaPuerta != null)
            advertenciaPuerta.SetActive(mostrar);
        if (advertenciaPuertaPC != null)
            advertenciaPuertaPC.SetActive(mostrar);

        if (mostrar)
        {
            if (advertenciaPuerta != null)
                StartCoroutine(ParpadeaObjeto(advertenciaPuerta));
            if (advertenciaPuertaPC != null)
                StartCoroutine(ParpadeaObjeto(advertenciaPuertaPC));
        }
    }

    IEnumerator ParpadeaObjeto(GameObject obj)
    {
        var img = obj.GetComponent<UnityEngine.UI.Image>();
        if (img == null) yield break;

        while (obj != null && obj.activeSelf)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSecondsRealtime(0.15f);
        }

        if (img != null)
            img.enabled = true;
    }
}