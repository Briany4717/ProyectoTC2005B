using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la lógica y visualización del minijuego de preguntas.
/// </summary>
public class GLQuestionSetup : MonoBehaviour
{
    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GLAnswerButton[] answerButtons;
    [SerializeField] private int correctAnswerChoice;
    [SerializeField] private Image errorImage;

    StationData currentStation;

    /// <summary>
    /// Carga las preguntas y oculta la imagen de error.
    /// </summary>
    private void Awake()
    {
        GetQuestionAssets();
        errorImage.gameObject.SetActive(false);
    }

    void Start()
    {
    }

    /// <summary>
    /// Abre la pregunta actual al interactuar con una estación.
    /// </summary>
    public void OpenQuestion(StationData station)
    {
        currentStation = station;
        LoadNewQuestion();
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Ejecuta la lógica cuando se elige la respuesta correcta.
    /// </summary>
    public void CorrectAnswer()
    {
        GLMenusStationsManager.Instance.CloseAllMenus();
        OrderManager.Instance.OnPlayerCompletedStation(currentStation);
    }

    /// <summary>
    /// Ejecuta la lógica de error y reproducción de sonido al fallar.
    /// </summary>
    public void WrongAnswer()
    {
        Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.JeopardyFailure);
        ShowError();
    }

    /// <summary>
    /// Muestra un indicador visual de error por un tiempo breve.
    /// </summary>
    public void ShowError()
    {
        errorImage.gameObject.SetActive(true);
        CancelInvoke("HideError");
        Invoke("HideError", 1f);
    }

    /// <summary>
    /// Oculta el indicador visual de error.
    /// </summary>
    public void HideError()
    {
        errorImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Prepara y muestra una nueva pregunta en pantalla.
    /// </summary>
    public void LoadNewQuestion()
    {
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    /// <summary>
    /// Obtiene los datos de las preguntas de la carpeta de recursos.
    /// </summary>
    private void GetQuestionAssets()
    {
        questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    /// <summary>
    /// Selecciona aleatoriamente una pregunta de la lista y la elimina.
    /// </summary>
    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }

    /// <summary>
    /// Configura el texto de la pregunta actual.
    /// </summary>
    private void SetQuestionValues()
    {
        questionText.text = currentQuestion.question;
    }

    /// <summary>
    /// Configura los botones con respuestas aleatorizadas.
    /// </summary>
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

    /// <summary>
    /// Aleatoriza el orden de las respuestas para evitar patrones.
    /// </summary>
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