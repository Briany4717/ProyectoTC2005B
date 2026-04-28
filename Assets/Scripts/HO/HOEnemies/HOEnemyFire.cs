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

    // Update is called once per frame
    void Update()
    {
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
        Instantiate(balaEnemigo, controladorlDisparo.position, controladorlDisparo.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorlDisparo.position, controladorlDisparo.position + transform.right * distanciaLinea);
    }
}
