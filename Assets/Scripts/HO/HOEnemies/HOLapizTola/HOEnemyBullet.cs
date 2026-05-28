using UnityEngine;


/// Define el comportamiento de los proyectiles disparados por los enemigos.

public class HOEnemyBullet : MonoBehaviour
{
    public float velocidad;

    public int danio;

    
    /// Mueve el proyectil hacia la derecha a una velocidad constante.
    
    void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * Vector2.right);
    }

    
    /// Daña al jugador si colisiona con él y destruye el proyectil.
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HOPlayerLives vidaJugador))
        {
            vidaJugador.TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}