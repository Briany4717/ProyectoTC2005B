using UnityEngine;

public class RapidezPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    public void Interact()
    {
        completeStation();
    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }


    void completeStation()
    {
        OrderManager.Instance.OnPlayerCompletedStation(station);
    }
}
