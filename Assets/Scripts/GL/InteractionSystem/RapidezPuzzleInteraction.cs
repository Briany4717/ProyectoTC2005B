using UnityEngine;

public class RapidezPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private RapidezControler rapidezControler;
    public void Interact()
    {
        // iniciamos el juego y le pasamos la referencia de la estacion para que sepa que completar
        rapidezControler.SetRapidezGame(station);
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
