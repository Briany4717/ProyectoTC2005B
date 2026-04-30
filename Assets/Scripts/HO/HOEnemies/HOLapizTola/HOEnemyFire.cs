using UnityEngine;

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

    void Start()
    {
        armaScript = GetComponent<HOEnemyVerticalFollow>();
    }

    // Update is called once per frame
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

    private void Disparar()
    {
        GameObject bala = Instantiate(balaEnemigo, controladorlDisparo.position, controladorlDisparo.rotation);
        
        HOEnemyBullet bulletScript = bala.GetComponent<HOEnemyBullet>();
        bulletScript.danio = armaScript.danioActual;
        bulletScript.velocidad = armaScript.velocidadBalaActual;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorlDisparo.position, controladorlDisparo.position + transform.right * distanciaLinea);
    }
    */
}
