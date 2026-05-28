using UnityEngine;

/// <summary>
/// Contiene la pregunta y sus posibles respuestas para el minijuego de preguntas.
/// </summary>
[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData")]
public class QuestionData : ScriptableObject
{
    public string question;
    [Tooltip("The correct answer should always be listed first")]
    public string[] answers;
}