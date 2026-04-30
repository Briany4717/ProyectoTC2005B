using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Globalization;
using System.Text;
using System;
using UnityEngine.EventSystems;

public class RapidezControler : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI wordOutput;
    [SerializeField] public TMP_InputField inputField;  // Campo de entrada de texto
    [SerializeField] public GLWordBank wordBank = null;

    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;
    private bool isGameActive = false;

    StationData currentStation;

    private void Start()
    {
        // SetCurrenWord();
    }

    public void SetRapidezGame(StationData station)
    {
        currentStation = station;
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Rapidez);
        SetCurrenWord();

        // Activamos el campo de entrada y nos suscribimos a sus cambios
        if (inputField != null)
        {
            isGameActive = true;
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            inputField.ActivateInputField();
            inputField.text = string.Empty;
        }
    }

    private void Update()
    {
        // Si el juego está activo y el InputField no está seleccionado, reactivarlo
        if (isGameActive && inputField != null)
        {
            if (EventSystem.current.currentSelectedGameObject != inputField.gameObject)
            {
                inputField.ActivateInputField();
            }
        }
    }

    private void SetCurrenWord()
    {
        currentWord = wordBank.GetPrompt();
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newString)
    {
        remainingWord = NormalizeText(newString);
        wordOutput.text = remainingWord;
    }

    private void OnDisable()
    {
        isGameActive = false;
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
    }

    // Se llama cada vez que el texto del campo cambia
    private void OnInputFieldValueChanged(string newText)
    {
        if (string.IsNullOrEmpty(newText)) return;

        // Obtén el último carácter escrito
        string lastCharacter = NormalizeText(newText.Substring(newText.Length - 1));

        if (IsCorrectLetter(lastCharacter))
        {
            RemoveLetter();

            // Limpiamos el campo de entrada
            inputField.text = string.Empty;

            if (IsWordComplete())
                EndGame();
            else
            {
                // Reactivar el InputField para la siguiente letra
                inputField.ActivateInputField();
            }
        }
        else
        {
            // Si no es correcto, limpiamos el campo para que intente de nuevo
            inputField.text = string.Empty;
            // Reactivar el InputField
            inputField.ActivateInputField();
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        // Desuscribimos antes de cerrar
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }

        // cerramos todas las pantallas
        GLMenusStationsManager.Instance.CloseAllMenus();
        // avisamos que completamos la estacion
        OrderManager.Instance.OnPlayerCompletedStation(currentStation);
    }

    private bool IsCorrectLetter(string letter)
    {
        if (string.IsNullOrEmpty(remainingWord)) return false;

        // Obtener el primer carácter de la palabra restante
        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);

        return string.Equals(firstLetter, letter, StringComparison.Ordinal);
    }

    private void RemoveLetter()
    {
        if (string.IsNullOrEmpty(remainingWord)) return;

        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        remainingWord = remainingWord.Substring(firstLetter.Length);
        wordOutput.text = remainingWord;
    }

    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

    private static string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Normalize(NormalizationForm.FormC);
    }

}