using UnityEngine;

/// <summary>
/// Destruye el objeto después de un tiempo definido.
/// </summary>
public class HOLifeTime : MonoBehaviour
{
    public float tiempoDeVida;

    /// <summary>
    /// Programa la destrucción del objeto al iniciar.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}