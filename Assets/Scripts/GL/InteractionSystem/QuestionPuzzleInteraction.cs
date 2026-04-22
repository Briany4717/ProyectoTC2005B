using UnityEngine;

public class QuestionPuzzleInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        TestInteract();
    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }


    void TestInteract()
    {

        print("interactuaste!");

    }
}
