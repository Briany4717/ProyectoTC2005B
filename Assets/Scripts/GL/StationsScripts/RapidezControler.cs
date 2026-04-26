using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.Build.Player;
using UnityEditor.XR;
using Unity.VisualScripting;

public class RapidezControler : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI wordOutput;
    [SerializeField] public GLWordBank wordBank = null;

    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;

    private void Start()
    {
        SetCurrenWord();
    }

    private void SetCurrenWord()
    {
        currentWord = wordBank.GetPrompt();
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newString)
    {
        remainingWord = newString;
        wordOutput.text = remainingWord;
    }

    // Creamos un evento para recibir el input
    private void OnEnable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += CheckInput;
        }
    }


    // Nos desuscribimos
    private void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= CheckInput;
        }
    }

    // Usamos Update para las teclas especiales (Backspace y Enter)
    private void Update()
    {
        // if (Keyboard.current == null) return;

        // // Comprobar Backspace
        // if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        // {
        //     if (typedText.Length > 0)
        //     {
        //         typedText = typedText.Substring(0, typedText.Length - 1);
        //         typedTextUI.text = typedText;
        //     }
        // }

        // // Comprobar Enter (Validamos tanto el Enter normal como el del teclado numérico)
        // if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        // {
        //     if (typedText == targetText)
        //     {
        //         resultTextUI.text = "¡Correcto!";
        //         // GLMenusStationsManager.Instance.CloseAllMenus(); // Lógica de victoria
        //     }
        //     else
        //     {
        //         resultTextUI.text = "Incorrecto";
        //     }
        // }
    }

    // Usamos onTextInput SOLO para las letras y símbolos
    private void CheckInput(char character)
    {
        // Ignoramos cualquier caracter de control invisible (Tab, Esc, y por si acaso, Enter/Backspace)
        if (char.IsControl(character)) return;

        // Agregamos la letra
        string charPressed = character.ToString();
        EnterLetter(charPressed);
    }

    private void EnterLetter(string typedLetter)
    {
        if (IsCorrectLetter(typedLetter))
        {
            RemoveLetter();

            // en este if se pone la logica de cuando se completa la palabra
            if (IsWordComplete())
                SetCurrenWord();
        }
    }

    private bool IsCorrectLetter(string letter)
    {
        // check if the next one is the first one
        return remainingWord.IndexOf(letter) == 0;
    }

    private void RemoveLetter()
    {
        string newString = remainingWord.Remove(0, 1);
        SetRemainingWord(newString);
    }
    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

}
