using TMPro;
using UnityEngine;

public class GLAnswerButton : MonoBehaviour
{
    private bool isCorrect;
    [SerializeField] TextMeshProUGUI answerText;

    public void SetAnswerText(string text)
    {
        answerText.text = text;
    }

    public void SetIsCorrect(bool newBool)
    {
        isCorrect = newBool;
    }

    public void OnClick()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer!!");
            GLQuestionSetup.Instance.LoadNewQuestion();
        }
        else
        {
            Debug.Log("Wrong Answer!!");
        }
    }

}
