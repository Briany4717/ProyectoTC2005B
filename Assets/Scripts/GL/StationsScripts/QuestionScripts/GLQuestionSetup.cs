using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GLQuestionSetup : MonoBehaviour
{


    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GLAnswerButton[] answerButtons;
    [SerializeField] private int correctAnswerChoice;
    [SerializeField] private Image errorImage;








    //station we are trying to do
    StationData currentStation;

    private void Awake()
    {
        GetQuestionAssets();
        errorImage.gameObject.SetActive(false);

    }

    void Start()
    {

    }

    // when interact with station start the jeopardy
    public void OpenQuestion(StationData station)
    {
        currentStation = station;
        LoadNewQuestion();
        this.gameObject.SetActive(true);

    }

    public void CorrectAnswer()
    {

        // Avisamos al manager general que cierre todos los menús de las estaciones
        GLMenusStationsManager.Instance.CloseAllMenus();

        // Avisamos al OrderManager que completamos la estación
        OrderManager.Instance.OnPlayerCompletedStation(currentStation);
    }

    public void WrongAnswer()
    {
        // Aquí podríamos añadir alguna penalización o mensaje de error
        Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");

        ShowError();

    }


    // funcion para aparecer una cruz cuando se equivocan
    public void ShowError()
    {
        errorImage.gameObject.SetActive(true);
        CancelInvoke("HideError"); // Cancelamos cualquier invocación pendiente de HideError
        Invoke("HideError", 1f); // Oculta la cruz después de 1 segundo
    }
    public void HideError()
    {
        errorImage.gameObject.SetActive(false);
    }

    public void LoadNewQuestion()
    {
        // FUNCTIONS IN CHARGED OF SETING UP ALL THE STUFF OF THE JEOPARDY
        // Get a new question
        SelectNewQuestion();
        // set all text and values on screen
        SetQuestionValues();
        // set all the answer buttons text and correct answer values
        SetAnswerValues();

    }

    private void GetQuestionAssets()
    {
        // get all of the questions from the question folder
        questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }

    private void SetQuestionValues()
    {
        questionText.text = currentQuestion.question;
    }

    private void SetAnswerValues()
    {
        List<string> answers = RandomizeAnswer(new List<string>(currentQuestion.answers));


        // setup each buttons for the list of answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = false;
            //  if it is the correct answer set the bool to true
            if (i == correctAnswerChoice)
            {
                isCorrect = true;
            }
            answerButtons[i].SetupButton(answers[i], isCorrect, this);
        }
    }

    // randomize the correct answer from the buttons to never have the same ones 
    // in the same order
    private List<string> RandomizeAnswer(List<string> originalList)
    {
        bool correctAnswerChosen = false;
        List<string> newList = new List<string>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            // get a random number of the remaining choices (buttons)
            int randomButton = Random.Range(0, originalList.Count);

            // check if we got 0 for the first time
            if (randomButton == 0 && !correctAnswerChosen)
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }

            // add this to the new list
            newList.Add(originalList[randomButton]);
            originalList.RemoveAt(randomButton);
        }

        return newList;
    }

}
