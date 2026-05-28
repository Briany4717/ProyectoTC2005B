using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BotonPuerta : MonoBehaviour
{
    [Header("Configuración")]
    public float rangoActivacion = 1.5f;
    public Transform jugador;

    [Header("Sprites de la puerta")]
    public SpriteRenderer spritePuerta;
    public Sprite[] framesCerrar;
    public Sprite spriteAbierta;

    [Header("Velocidad animación")]
    public float fps = 10f;

    

    private bool presionando = false;
    private bool animacionTerminada = false;
    private bool rayoConsumido = false;
    private Coroutine coroutineActual;

    void Update()
    {
        float distancia = Mathf.Abs(jugador.position.x - transform.position.x);
        bool dentroDelRango = distancia <= rangoActivacion;
        bool presionandoE = Keyboard.current.eKey.isPressed;

        if (dentroDelRango && presionandoE)
        {
            if (!presionando)
            {
                // Verificar si tiene rayos antes de empezar
                if (!SistemaRayos.instancia.UsarRayo())
                {
                    Debug.Log("No tienes rayos para cerrar la puerta");
                    return;
                }
                MusicController.instancia?.PlayDoorSlam();
                rayoConsumido = true;
                presionando = true;
                animacionTerminada = false;
                if (coroutineActual != null) StopCoroutine(coroutineActual);
                coroutineActual = StartCoroutine(AnimarCierre());
            }
        }
        else
        {
            if (presionando)
            {
                presionando = false;
                animacionTerminada = false;
                rayoConsumido = false;
                if (coroutineActual != null) StopCoroutine(coroutineActual);
                spritePuerta.sprite = spriteAbierta;
            }
        }
    }

    IEnumerator AnimarCierre()
    {
        float intervalo = 1f / fps;
        foreach (Sprite frame in framesCerrar)
        {
            if (!presionando) yield break;
            spritePuerta.sprite = frame;
            yield return new WaitForSeconds(intervalo);
        }
        animacionTerminada = true;
        spritePuerta.sprite = framesCerrar[framesCerrar.Length - 1];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoActivacion);
    }

    public bool EstaAnimacionTerminada()
    {
        return animacionTerminada;
    }
}