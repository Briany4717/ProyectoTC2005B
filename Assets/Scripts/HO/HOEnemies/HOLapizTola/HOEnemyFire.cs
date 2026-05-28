using UnityEngine;

/// <summary>
/// Maneja la detección del jugador y el disparo del enemigo.
/// </summary>
public class HOEnemyFire : MonoBehaviour
{
    public Transform controladorlDisparo;
    public float distanciaLinea;
    public LayerMask capaJugador;
    public bool jugadorEnRango;
    public float tiempoEntreDisparos;
    public float tiempoUltimoDisparo;
    public GameObject balaEnemigo;
    public float tiempoEsperaDisparo;

    private HOEnemyVerticalFollow armaScript;

    /// <summary>
    /// Obtiene la referencia al script de movimiento vertical.
    /// </summary>
    void Start()
    {
        armaScript = GetComponent<HOEnemyVerticalFollow>();
    }

    /// <summary>
    /// Detecta si el jugador está en rango y dispara si ha pasado el tiempo necesario.
    /// </summary>
    void Update()
    {
        if (armaScript != null && armaScript.IsEntering)
        {
            return;
        } 
        jugadorEnRango = Physics2D.Raycast(controladorlDisparo.position, transform.right, distanciaLinea, capaJugador);

        if (jugadorEnRango)
        {
            if (Time.time > tiempoEntreDisparos + tiempoUltimoDisparo)
            {
                tiempoUltimoDisparo = Time.time;
                Invoke(nameof(Disparar), tiempoEsperaDisparo);
            }
        }
    }

    /// <summary>
    /// Instancia la bala y le asigna las estadísticas de daño y velocidad.
    /// </summary>
    private void Disparar()
    {
        GameObject bala = Instantiate(balaEnemigo, controladorlDisparo.position, controladorlDisparo.rotation);
        
        HOEnemyBullet bulletScript = bala.GetComponent<HOEnemyBullet>();
        bulletScript.danio = armaScript.danioActual;
        bulletScript.velocidad = armaScript.velocidadBalaActual;
    }
}