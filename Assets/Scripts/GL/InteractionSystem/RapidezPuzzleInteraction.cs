using UnityEngine;

/// <summary>
/// Permite al jugador interactuar con la estación del minijuego de rapidez.
/// </summary>
public class RapidezPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private RapidezControler rapidezControler;

    /// <summary>
    /// Inicia el juego de mecanografía rápida y le pasa la referencia de la estación.
    /// </summary>
    public void Interact()
    {
        rapidezControler.SetRapidezGame(station);
    }

    /// <summary>
    /// Evento que se ejecuta al dejar de estar en rango de interacción.
    /// </summary>
    public void OnNotTouchingPlayer()
    {
    }

    /// <summary>
    /// Evento que se ejecuta al estar en rango de interacción.
    /// </summary>
    public void OnTouchingPlayer()
    {
    }

    /// <summary>
    /// Lógica ejecutada al completar exitosamente la estación de rapidez.
    /// </summary>
    void completeStation()
    {
    }
}