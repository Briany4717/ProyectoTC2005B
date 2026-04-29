using TMPro;
using UnityEngine;

public class GLAnswerButton : MonoBehaviour
{
    private bool isCorrect;
    [SerializeField] TextMeshProUGUI answerText;


    // Guardamos la referencia del manager de la pregunta
    private GLQuestionSetup questionSetup;

    // Poner los valores al boton
    public void SetupButton(string text, bool newBool, GLQuestionSetup setup)
    {
        answerText.text = text;
        isCorrect = newBool;
        questionSetup = setup; // Guardamos quién creó esta pregunta
    }
    public void OnClick()
    {
        if (isCorrect)
        {
            questionSetup.CorrectAnswer();
        }
        else
        {

            questionSetup.WrongAnswer();
        }
    }

}
