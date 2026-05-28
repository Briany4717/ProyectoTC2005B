using UnityEngine;


/// Destruye el objeto después de un tiempo definido.

public class HOLifeTime : MonoBehaviour
{
    public float tiempoDeVida;

    
    /// Programa la destrucción del objeto al iniciar.
    
    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}