using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Globalization;
using System.Text;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Controlador de la estación de juego de mecanografía rápida.
/// </summary>
public class RapidezControler : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI wordOutput;
    [SerializeField] public TMP_InputField inputField;
    [SerializeField] public TextMeshProUGUI tituloField;
    [SerializeField] public GLWordBank wordBank = null;

    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;
    private bool isGameActive = false;

    StationData currentStation;

    private void Start()
    {
    }

    /// <summary>
    /// Configura y comienza el juego de rapidez para una estación dada.
    /// </summary>
    public void SetRapidezGame(StationData station)
    {
        currentStation = station;
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Rapidez);
        StartCoroutine(InitializeGameWhenReady());
    }

    /// <summary>
    /// Espera a que el banco de palabras esté listo antes de inicializar el juego.
    /// </summary>
    private IEnumerator InitializeGameWhenReady()
    {
        if (wordBank == null)
        {
            Debug.LogError("Word bank is not assigned.");
            yield break;
        }

        yield return new WaitUntil(() => wordBank.IsReady);
        SetCurrenWord();

        if (inputField != null)
        {
            isGameActive = true;
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            inputField.ActivateInputField();
            inputField.text = string.Empty;
        }
    }

    /// <summary>
    /// Mantiene el enfoque en el campo de entrada mientras el juego está activo.
    /// </summary>
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

    /// <summary>
    /// Obtiene y configura la palabra actual a escribir.
    /// </summary>
    private void SetCurrenWord()
    {
        PromptData prompt = wordBank.GetPrompt();
        //SetRemainingWord(prompt.contenido, prompt.titulo);
    }

    /// <summary>
    /// Inicializa la visualización de la palabra restante y su título.
    /// </summary>
    private void SetRemainingWord(string newString, string titulo)
    {
        currentWord = NormalizeText(newString);
        remainingWord = currentWord;

        UpdateWordDisplay();

        tituloField.text = titulo;
    }

    /// <summary>
    /// Finaliza la actividad y limpia los eventos del campo de entrada.
    /// </summary>
    private void OnDisable()
    {
        isGameActive = false;
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
    }

    /// <summary>
    /// Maneja los cambios de texto en el campo de entrada.
    /// </summary>
    private void OnInputFieldValueChanged(string newText)
    {
        if (string.IsNullOrEmpty(newText)) return;

        string lastCharacter = NormalizeText(newText.Substring(newText.Length - 1));

        if (IsCorrectLetter(lastCharacter))
        {
            RemoveLetter();
            inputField.text = string.Empty;

            if (IsWordComplete())
                EndGame();
            else
            {
                inputField.ActivateInputField();
            }
        }
        else
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }
    }

    /// <summary>
    /// Finaliza el minijuego de rapidez y notifica el éxito.
    /// </summary>
    private void EndGame()
    {
        isGameActive = false;
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }

        GLMenusStationsManager.Instance.CloseAllMenus();
        OrderManager.Instance.OnPlayerCompletedStation(currentStation);
    }

    /// <summary>
    /// Verifica si la letra ingresada es correcta.
    /// </summary>
    private bool IsCorrectLetter(string letter)
    {
        if (string.IsNullOrEmpty(remainingWord)) return false;

        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        return string.Equals(firstLetter, letter, StringComparison.Ordinal);
    }

    /// <summary>
    /// Elimina la primera letra de la palabra restante y actualiza la vista.
    /// </summary>
    private void RemoveLetter()
    {
        if (string.IsNullOrEmpty(remainingWord)) return;

        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        remainingWord = remainingWord.Substring(firstLetter.Length);

        UpdateWordDisplay();
    }

    /// <summary>
    /// Actualiza la visualización de la palabra resaltando las letras correctas.
    /// </summary>
    private void UpdateWordDisplay()
    {
        int typedLength = currentWord.Length - remainingWord.Length;
        string typedPart = currentWord.Substring(0, typedLength);
        wordOutput.text = $"<color=green>{typedPart}</color>{remainingWord}";
    }

    /// <summary>
    /// Comprueba si se ha terminado de escribir la palabra.
    /// </summary>
    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

    /// <summary>
    /// Normaliza el texto ingresado.
    /// </summary>
    private static string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Normalize(NormalizationForm.FormC);
    }
}