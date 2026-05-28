using UnityEngine;

/// <summary>
/// Gestiona la vida de los enemigos y sus recompensas al morir.
/// </summary>
public class HOEnemyLives : MonoBehaviour
{
    public float lives;

    /// <summary>
    /// Reduce la vida del enemigo y verifica si muere.
    /// </summary>
    public void TomarDanio(float danio)
    {
        lives -= danio;
        if (lives <= 0)
        {
            death();
        }
    }

    /// <summary>
    /// Maneja la muerte del enemigo, otorgando recompensas y notificando.
    /// </summary>
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