
/// Interfaz para enemigos que pueden escalar su dificultad.

public interface IHOScalableEnemy
{
    
    /// Ajusta las estadísticas del enemigo según el nivel de dificultad.
    
    void SetDifficulty(int level);
}