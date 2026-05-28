using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Permite al jugador interactuar con la estación del minijuego de preguntas.
/// </summary>
public class QuestionPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private GLQuestionSetup questionSetup;

    /// <summary>
    /// Abre el menú del minijuego de preguntas y carga una pregunta al interactuar.
    /// </summary>
    public void Interact()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Question);
        questionSetup.OpenQuestion(station);
    }

    /// <summary>
    /// Evento disparado cuando el jugador se aleja de la estación.
    /// </summary>
    public void OnNotTouchingPlayer()
    {
    }

    /// <summary>
    /// Evento disparado cuando el jugador se acerca a la estación.
    /// </summary>
    public void OnTouchingPlayer()
    {
    }
}