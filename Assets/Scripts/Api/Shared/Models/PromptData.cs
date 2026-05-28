using UnityEngine;

/// <summary>
/// Modelo de datos para almacenar la información de un prompt.
/// </summary>
public class PromptData
{
    /// <summary>
    /// Título del prompt.
    /// </summary>
    public string titulo { get; set; }

    /// <summary>
    /// Contenido o descripción del prompt.
    /// </summary>
    public string contenido { get; set; }

    /// <summary>
    /// Categoría a la que pertenece el prompt.
    /// </summary>
    public string categoria { get; set; }
}
