using UnityEngine;


/// Gestiona la vida de los enemigos y sus recompensas al morir.

public class HOEnemyLives : MonoBehaviour
{
    public float lives;

    
    /// Reduce la vida del enemigo y verifica si muere.
    
    public void TomarDanio(float danio)
    {
        lives -= danio;
        if (lives <= 0)
        {
            death();
        }
    }

    
    /// Maneja la muerte del enemigo, otorgando recompensas y notificando.
    
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