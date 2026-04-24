using UnityEngine;

public class ControlPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    public void Interact()
    {
        Debug.Log("Estacion Control");
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
