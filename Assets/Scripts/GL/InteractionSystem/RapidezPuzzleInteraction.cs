using UnityEngine;


/// Permite al jugador interactuar con la estación del minijuego de rapidez.

public class RapidezPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private RapidezControler rapidezControler;

    
    /// Inicia el juego de mecanografía rápida y le pasa la referencia de la estación.
    
    public void Interact()
    {
        rapidezControler.SetRapidezGame(station);
    }

    
    /// Evento que se ejecuta al dejar de estar en rango de interacción.
    
    public void OnNotTouchingPlayer()
    {
    }

    
    /// Evento que se ejecuta al estar en rango de interacción.
    
    public void OnTouchingPlayer()
    {
    }

    
    /// Lógica ejecutada al completar exitosamente la estación de rapidez.
    
    void completeStation()
    {
    }
}