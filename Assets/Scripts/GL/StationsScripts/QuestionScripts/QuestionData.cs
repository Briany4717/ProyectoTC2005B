using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData")]
public class QuestionData : ScriptableObject
{
    public string question;
    [Tooltip("The correct answer should always be listed first")]
    public string[] answers;

}
