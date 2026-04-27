using Unity.VisualScripting;
using UnityEngine;

public class QuestionPuzzleInteraction : MonoBehaviour, IInteractable
{

    public StationData station;
    [SerializeField] private GLQuestionSetup questionSetup;

    public void Interact()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Question);
        questionSetup.OpenQuestion(station);
    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }

}
