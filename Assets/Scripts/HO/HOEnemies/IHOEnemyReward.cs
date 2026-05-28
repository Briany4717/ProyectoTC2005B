/// <summary>
/// Interfaz para obtener recompensas de un enemigo derrotado.
/// </summary>
public interface IHOEnemyReward
{
    /// <summary>
    /// Devuelve la cantidad de monedas otorgadas.
    /// </summary>
    int GetCoinsReward();
    
    /// <summary>
    /// Devuelve el tiempo extra otorgado.
    /// </summary>
    float GetTimeReward();
}