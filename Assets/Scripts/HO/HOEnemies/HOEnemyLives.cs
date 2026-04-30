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
        IHOEnemyReward reward = GetComponent<IHOEnemyReward>();
        if (reward != null)
        {
            if (HOCoins.Instance != null)
            {
                HOCoins.Instance.AddCoins(reward.GetCoinsReward());
            }
            if (HOTimer.Instance != null)
            {
                HOTimer.Instance.AddTime(reward.GetTimeReward());
            }
        }
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnEnemyDefeated();
        }

        Destroy(gameObject);
    }
}