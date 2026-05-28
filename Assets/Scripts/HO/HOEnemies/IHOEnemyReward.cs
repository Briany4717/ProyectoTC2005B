
/// Interfaz para obtener recompensas de un enemigo derrotado.

public interface IHOEnemyReward
{
    
    /// Devuelve la cantidad de monedas otorgadas.
    
    int GetCoinsReward();
    
    
    /// Devuelve el tiempo extra otorgado.
    
    float GetTimeReward();
}