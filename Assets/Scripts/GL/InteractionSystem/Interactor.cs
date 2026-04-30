using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}

public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();

            GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.Interaction);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();
        }
    }
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
