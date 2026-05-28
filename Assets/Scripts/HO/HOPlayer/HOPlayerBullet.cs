using UnityEngine;


/// Define el comportamiento del proyectil disparado por el jugador.

public class HOPlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float danio;

    
    /// Mueve el proyectil horizontalmente según su velocidad.
    
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    
    /// Daña a los enemigos con los que colisiona y luego se destruye.
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HOEnemy"))
        {
            collision.GetComponent<HOEnemyLives>().TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}