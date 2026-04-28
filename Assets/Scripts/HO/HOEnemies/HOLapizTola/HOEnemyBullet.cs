using UnityEngine;

public class HOEnemyBullet : MonoBehaviour
{
    public float velocidad;

    public int danio;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HOPlayerLives vidaJugador))
        {
            vidaJugador.TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}
