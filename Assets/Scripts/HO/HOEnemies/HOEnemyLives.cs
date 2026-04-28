using UnityEngine;

public class HOEnemyLives : MonoBehaviour
{
    public float lives;
    // public GameObject deathEffect;

    public void TomarDanio(float danio)
    {
        lives -= danio;
        if (lives <= 0)
        {
            death();
        }
    }

    void death()
    {
        // Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}