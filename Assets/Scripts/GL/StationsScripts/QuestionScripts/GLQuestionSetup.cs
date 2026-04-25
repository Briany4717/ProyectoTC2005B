using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GLQuestionSetup : MonoBehaviour
{
    public static GLQuestionSetup Instance;
    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GLAnswerButton[] answerButtons;
    [SerializeField] private int correctAnswerChoice;

    private void Awake()
    {
        Instance = this;
        GetQuestionAssets();
    }

    void Start()
    {
        LoadNewQuestion();
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

    // Update is called once per frame
    void Update()
    {

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
            answerButtons[i].SetIsCorrect(isCorrect);
            answerButtons[i].SetAnswerText(answers[i]);
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
