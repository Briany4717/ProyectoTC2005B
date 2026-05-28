using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// Gestiona la lógica y visualización del minijuego de preguntas.

public class GLQuestionSetup : MonoBehaviour
{
    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GLAnswerButton[] answerButtons;
    [SerializeField] private int correctAnswerChoice;
    [SerializeField] private Image errorImage;

    StationData currentStation;

    
    /// Carga las preguntas y oculta la imagen de error.
    
    private void Awake()
    {
        GetQuestionAssets();
        errorImage.gameObject.SetActive(false);
    }

    void Start()
    {
    }

    
    /// Abre la pregunta actual al interactuar con una estación.
    
    public void OpenQuestion(StationData station)
    {
        currentStation = station;
        LoadNewQuestion();
        this.gameObject.SetActive(true);
    }

    
    /// Ejecuta la lógica cuando se elige la respuesta correcta.
    
    public void CorrectAnswer()
    {
        GLMenusStationsManager.Instance.CloseAllMenus();
        OrderManager.Instance.OnPlayerCompletedStation(currentStation);
    }

    
    /// Ejecuta la lógica de error y reproducción de sonido al fallar.
    
    public void WrongAnswer()
    {
        Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.JeopardyFailure);
        ShowError();
    }

    
    /// Muestra un indicador visual de error por un tiempo breve.
    
    public void ShowError()
    {
        errorImage.gameObject.SetActive(true);
        CancelInvoke("HideError");
        Invoke("HideError", 1f);
    }

    
    /// Oculta el indicador visual de error.
    
    public void HideError()
    {
        errorImage.gameObject.SetActive(false);
    }

    
    /// Prepara y muestra una nueva pregunta en pantalla.
    
    public void LoadNewQuestion()
    {
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    
    /// Obtiene los datos de las preguntas de la carpeta de recursos.
    
    private void GetQuestionAssets()
    {
        questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    
    /// Selecciona aleatoriamente una pregunta de la lista y la elimina.
    
    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }

    
    /// Configura el texto de la pregunta actual.
    
    private void SetQuestionValues()
    {
        questionText.text = currentQuestion.question;
    }

    
    /// Configura los botones con respuestas aleatorizadas.
    
    private void SetAnswerValues()
    {
        List<string> answers = RandomizeAnswer(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = false;
            if (i == correctAnswerChoice)
            {
                isCorrect = true;
            }
            answerButtons[i].SetupButton(answers[i], isCorrect, this);
        }
    }

    
    /// Aleatoriza el orden de las respuestas para evitar patrones.
    
    private List<string> RandomizeAnswer(List<string> originalList)
    {
        bool correctAnswerChosen = false;
        List<string> newList = new List<string>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int randomButton = Random.Range(0, originalList.Count);

            if (randomButton == 0 && !correctAnswerChosen)
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }

            newList.Add(originalList[randomButton]);
            originalList.RemoveAt(randomButton);
        }

        return newList;
    }
}