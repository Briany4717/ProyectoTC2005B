using TMPro;
using UnityEngine;


/// Controla el comportamiento y los datos de un botón de respuesta en el minijuego de preguntas.

public class GLAnswerButton : MonoBehaviour
{
    private bool isCorrect;
    [SerializeField] TextMeshProUGUI answerText;

    private GLQuestionSetup questionSetup;

    
    /// Configura el texto, la validez de la respuesta y la referencia al controlador principal.
    
    public void SetupButton(string text, bool newBool, GLQuestionSetup setup)
    {
        answerText.text = text;
        isCorrect = newBool;
        questionSetup = setup;
    }

    
    /// Procesa la selección de esta respuesta, notificando el acierto o penalizando el tiempo por error.
    
    public void OnClick()
    {
        if (isCorrect)
        {
            questionSetup.CorrectAnswer();
        }
        else
        {
            questionSetup.WrongAnswer();
            GLGameControl.Instance.RemoveTime(5f);
        }
    }
}