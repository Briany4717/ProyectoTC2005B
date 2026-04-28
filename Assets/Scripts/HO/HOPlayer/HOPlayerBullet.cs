using UnityEngine;

public class HOPlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float danio;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HOEnemy"))
        {
            collision.GetComponent<HOEnemyLives>().TomarDanio(danio);
            Destroy(gameObject);
        }
    }
}
