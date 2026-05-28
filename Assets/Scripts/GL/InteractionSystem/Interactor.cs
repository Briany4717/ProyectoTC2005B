using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


/// Define los métodos necesarios para los objetos interactuables en el entorno.

public interface IInteractable
{
    void Interact();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}


/// Detecta y ejecuta las interacciones del jugador con los objetos interactuables cercanos.

public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable;

    
    /// Verifica si el jugador presiona la tecla de interacción y ejecuta la acción.
    
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
            GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.Interaction);
        }
    }

    
    /// Detecta cuando el jugador se acerca a un objeto interactuable.
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();
        }
    }

    
    /// Detecta cuando el jugador se aleja de un objeto interactuable y lo deselecciona.
    
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