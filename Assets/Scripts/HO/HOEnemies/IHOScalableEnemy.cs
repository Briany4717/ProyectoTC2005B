/// <summary>
/// Interfaz para enemigos que pueden escalar su dificultad.
/// </summary>
public interface IHOScalableEnemy
{
    /// <summary>
    /// Ajusta las estadísticas del enemigo según el nivel de dificultad.
    /// </summary>
    void SetDifficulty(int level);
}