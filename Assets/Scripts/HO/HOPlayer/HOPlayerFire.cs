using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla la acción de disparar del jugador.
/// </summary>
public class HOPlayerFire : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject proyectil;

    /// <summary>
    /// Detecta la entrada del jugador para disparar.
    /// </summary>
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shot();
        }
    }

    /// <summary>
    /// Instancia un proyectil en la posición y rotación del controlador de disparo.
    /// </summary>
    void shot()
    {
        Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
    }
}