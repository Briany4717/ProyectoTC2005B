using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.Build.Player;

public class RapidezControler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetTextUI;
    [SerializeField] private TextMeshProUGUI typedTextUI;
    [SerializeField] private TextMeshProUGUI resultTextUI;

    private string targetText = "hello";
    private string typedText = "";

    private void Start()
    {
        targetTextUI.text = "Escribe esto: " + targetText;
        typedTextUI.text = typedText;
        resultTextUI.text = "";
    }

    // Creamos un evento para recibir el input
    private void OnEnable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += HandleTextInput;
        }
    }


    // Nos desuscribimos
    private void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= HandleTextInput;
        }
    }

    // Usamos Update para las teclas especiales (Backspace y Enter)
    private void Update()
    {
        if (Keyboard.current == null) return;

        // Comprobar Backspace
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            if (typedText.Length > 0)
            {
                typedText = typedText.Substring(0, typedText.Length - 1);
                typedTextUI.text = typedText;
            }
        }

        // Comprobar Enter (Validamos tanto el Enter normal como el del teclado numérico)
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            if (typedText == targetText)
            {
                resultTextUI.text = "¡Correcto!";
                // GLMenusStationsManager.Instance.CloseAllMenus(); // Lógica de victoria
            }
            else
            {
                resultTextUI.text = "Incorrecto";
            }
        }
    }

    // Usamos onTextInput SOLO para las letras y símbolos
    private void HandleTextInput(char character)
    {
        // Ignoramos cualquier caracter de control invisible (Tab, Esc, y por si acaso, Enter/Backspace)
        if (char.IsControl(character)) return;

        // Agregamos la letra
        typedText += character;
        typedTextUI.text = typedText;
    }
}
