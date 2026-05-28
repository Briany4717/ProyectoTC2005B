using UnityEngine;
using UnityEngine.InputSystem;


/// Controla la acción de disparar del jugador.

public class HOPlayerFire : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject proyectil;

    
    /// Detecta la entrada del jugador para disparar.
    
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shot();
        }
    }

    
    /// Instancia un proyectil en la posición y rotación del controlador de disparo.
    
    void shot()
    {
        Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
    }
}