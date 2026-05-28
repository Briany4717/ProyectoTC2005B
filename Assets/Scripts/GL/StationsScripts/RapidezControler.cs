using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Globalization;
using System.Text;
using System;
using UnityEngine.EventSystems;


/// Controlador de la estación de juego de mecanografía rápida.

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

    
    /// Configura y comienza el juego de rapidez para una estación dada.
    
    public void SetRapidezGame(StationData station)
    {
        currentStation = station;
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Rapidez);
        StartCoroutine(InitializeGameWhenReady());
    }

    
    /// Espera a que el banco de palabras esté listo antes de inicializar el juego.
    
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

    
    /// Mantiene el enfoque en el campo de entrada mientras el juego está activo.
    
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

    
    /// Obtiene y configura la palabra actual a escribir.
    
    private void SetCurrenWord()
    {
        PromptData prompt = wordBank.GetPrompt();
        //SetRemainingWord(prompt.contenido, prompt.titulo);
    }

    
    /// Inicializa la visualización de la palabra restante y su título.
    
    private void SetRemainingWord(string newString, string titulo)
    {
        currentWord = NormalizeText(newString);
        remainingWord = currentWord;

        UpdateWordDisplay();

        tituloField.text = titulo;
    }

    
    /// Finaliza la actividad y limpia los eventos del campo de entrada.
    
    private void OnDisable()
    {
        isGameActive = false;
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
    }

    
    /// Maneja los cambios de texto en el campo de entrada.
    
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

    
    /// Finaliza el minijuego de rapidez y notifica el éxito.
    
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

    
    /// Verifica si la letra ingresada es correcta.
    
    private bool IsCorrectLetter(string letter)
    {
        if (string.IsNullOrEmpty(remainingWord)) return false;

        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        return string.Equals(firstLetter, letter, StringComparison.Ordinal);
    }

    
    /// Elimina la primera letra de la palabra restante y actualiza la vista.
    
    private void RemoveLetter()
    {
        if (string.IsNullOrEmpty(remainingWord)) return;

        StringInfo stringInfo = new StringInfo(remainingWord);
        string firstLetter = stringInfo.SubstringByTextElements(0, 1);
        remainingWord = remainingWord.Substring(firstLetter.Length);

        UpdateWordDisplay();
    }

    
    /// Actualiza la visualización de la palabra resaltando las letras correctas.
    
    private void UpdateWordDisplay()
    {
        int typedLength = currentWord.Length - remainingWord.Length;
        string typedPart = currentWord.Substring(0, typedLength);
        wordOutput.text = $"<color=green>{typedPart}</color>{remainingWord}";
    }

    
    /// Comprueba si se ha terminado de escribir la palabra.
    
    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

    
    /// Normaliza el texto ingresado.
    
    private static string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Normalize(NormalizationForm.FormC);
    }
}