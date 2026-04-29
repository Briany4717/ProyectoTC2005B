using System;
using Unity.VisualScripting;
using UnityEngine;

public class ControlPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private GLControlManager GLControlManager;
    public void Interact()
    {
        Debug.Log("Estacion Control");
        GLControlManager.StartControlGame();
    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }


    void completeStation()
    {
        // OrderManager.Instance.OnPlayerCompletedStation(station);
    }
}
