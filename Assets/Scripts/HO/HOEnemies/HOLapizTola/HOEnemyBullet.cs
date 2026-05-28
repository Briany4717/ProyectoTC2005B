using UnityEngine;

/// <summary>
/// Define el comportamiento de los proyectiles disparados por los enemigos.
/// </summary>
public class HOEnemyBullet : MonoBehaviour
{
    public float velocidad;

    public int danio;

    /// <summary>
    /// Mueve el proyectil hacia la derecha a una velocidad constante.
    /// </summary>
    void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * Vector2.right);
    }

    /// <summary>
    /// Daña al jugador si colisiona con él y destruye el proyectil.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HOPlayerLives vidaJugador))
        {
            vidaJugador.TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}