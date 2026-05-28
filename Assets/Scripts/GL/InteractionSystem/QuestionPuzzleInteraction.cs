using Unity.VisualScripting;
using UnityEngine;


/// Permite al jugador interactuar con la estación del minijuego de preguntas.

public class QuestionPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private GLQuestionSetup questionSetup;

    
    /// Abre el menú del minijuego de preguntas y carga una pregunta al interactuar.
    
    public void Interact()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Question);
        questionSetup.OpenQuestion(station);
    }

    
    /// Evento disparado cuando el jugador se aleja de la estación.
    
    public void OnNotTouchingPlayer()
    {
    }

    
    /// Evento disparado cuando el jugador se acerca a la estación.
    
    public void OnTouchingPlayer()
    {
    }
}