using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Define los métodos necesarios para los objetos interactuables en el entorno.
/// </summary>
public interface IInteractable
{
    void Interact();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}

/// <summary>
/// Detecta y ejecuta las interacciones del jugador con los objetos interactuables cercanos.
/// </summary>
public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable;

    /// <summary>
    /// Verifica si el jugador presiona la tecla de interacción y ejecuta la acción.
    /// </summary>
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
            GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.Interaction);
        }
    }

    /// <summary>
    /// Detecta cuando el jugador se acerca a un objeto interactuable.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();
        }
    }

    /// <summary>
    /// Detecta cuando el jugador se aleja de un objeto interactuable y lo deselecciona.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = null;
        }
    }
}