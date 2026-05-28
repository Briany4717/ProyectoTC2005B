using UnityEngine;

/// <summary>
/// Define el comportamiento del proyectil disparado por el jugador.
/// </summary>
public class HOPlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float danio;

    /// <summary>
    /// Mueve el proyectil horizontalmente según su velocidad.
    /// </summary>
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    /// <summary>
    /// Daña a los enemigos con los que colisiona y luego se destruye.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HOEnemy"))
        {
            collision.GetComponent<HOEnemyLives>().TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}