using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Globalization;
using System.Text;
using System;
using UnityEngine.EventSystems;

public class RapidezControler : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI wordOutput;
    [SerializeField] public TMP_InputField inputField;  // Campo de entrada de texto
    [SerializeField] public TextMeshProUGUI tituloField;
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
        StartCoroutine(InitializeGameWhenReady());
    }

    private IEnumerator InitializeGameWhenReady()
    {
        if (wordBank == null)
        {
            Debug.LogError("Word bank is not assigned.");
            yield break;
        }

        yield return new WaitUntil(() => wordBank.IsReady);
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
        PromptData prompt = wordBank.GetPrompt();
        //SetRemainingWord(prompt.contenido, prompt.titulo);
    }

    private void SetRemainingWord(string newString, string titulo)
    {
        currentWord = NormalizeText(newString);
        remainingWord = currentWord;

        UpdateWordDisplay();

        tituloField.text = titulo;
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
        // comparacion byte a byte
        return string.Equals(firstLetter, letter, StringComparison.Ordinal);
    }

    private void RemoveLetter()
    {
        if (string.IsNullOrEmpty(remainingWord)) return;

        // string que acepta unicode para manejar correctamente los caracteres compuestos
        StringInfo stringInfo = new StringInfo(remainingWord);
        // Obtener el primer carácter de la palabra restante
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        // Remover el primer carácter de la palabra restante
        remainingWord = remainingWord.Substring(firstLetter.Length);

        // Instead of wordOutput.text = remainingWord, call the display updater
        UpdateWordDisplay();
    }

    private void UpdateWordDisplay()
    {
        // Calculate how much of the string has been typed by comparing lengths
        int typedLength = currentWord.Length - remainingWord.Length;

        // Extract the typed portion and the untyped portion
        string typedPart = currentWord.Substring(0, typedLength);

        // Wrap the typed part in green color tags, followed by the remaining word
        wordOutput.text = $"<color=green>{typedPart}</color>{remainingWord}";
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