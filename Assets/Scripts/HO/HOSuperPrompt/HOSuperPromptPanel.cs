using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class PromptEx
{
    [TextArea] public string instruction;
    [TextArea] public string description;
    public string[] correctOrder;
}

/// <summary>
/// Gestiona la interfaz y la lógica de interacción del panel del Super Prompt.
/// </summary>
public class HOSuperPromptPanel : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI feedbackText;

    public Button[] promptButtons;
    public Button confirmButton;

    public PromptEx[] prompts;

    [Header("Colores de feedback")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color neutralColor = Color.white;

    private PromptEx currentPrompt;
    private string[] shuffledTexts;
    private List<int> selectionOrder;

    /// <summary>
    /// Configura los listeners de los botones al inicializar.
    /// </summary>
    void Awake()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            int index = i; 
            promptButtons[i].onClick.AddListener(() => OnPromptButtonClicked(index));
        }

        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    /// <summary>
    /// Prepara una nueva ronda cuando el panel se activa.
    /// </summary>
    void OnEnable()
    {
        SetupRound();
    }

    /// <summary>
    /// Configura los textos y mezcla las opciones de la pregunta actual.
    /// </summary>
    void SetupRound()
    {
        currentPrompt = prompts[0];

        selectionOrder = new List<int>();
        feedbackText.text = "";
        feedbackText.color = neutralColor;

        instructionText.text = currentPrompt.instruction;
        descriptionText.text = currentPrompt.description;

        shuffledTexts = ShuffleArray(currentPrompt.correctOrder);

        for (int i = 0; i < promptButtons.Length; i++)
        {
            if (i < shuffledTexts.Length)
            {
                SetButtonText(promptButtons[i], shuffledTexts[i]);
                promptButtons[i].gameObject.SetActive(true);
            }
            else
            {
                promptButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Maneja la selección y deselección de opciones por parte del usuario.
    /// </summary>
    void OnPromptButtonClicked(int buttonIndex)
    {
        if (selectionOrder.Contains(buttonIndex))
        {
            selectionOrder.Remove(buttonIndex);
        }
        else
        {
            selectionOrder.Add(buttonIndex);
        }

        UpdateButtonLabels();
    }

    /// <summary>
    /// Actualiza los textos de los botones para mostrar el orden de selección.
    /// </summary>
    void UpdateButtonLabels()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            string baseText = i < shuffledTexts.Length ? shuffledTexts[i] : "";
            int orderPosition = selectionOrder.IndexOf(i);

            if (orderPosition >= 0)
            {
                SetButtonText(promptButtons[i], $"[{orderPosition + 1}] {baseText}");
            }
            else
            {
                SetButtonText(promptButtons[i], baseText);
            }
        }
    }

    /// <summary>
    /// Verifica si el orden seleccionado es el correcto al confirmar.
    /// </summary>
    void OnConfirmClicked()
    {
        if (selectionOrder.Count < currentPrompt.correctOrder.Length)
        {
            feedbackText.text = "Selecciona todas las partes en orden";
            feedbackText.color = incorrectColor;
            return;
        }

        bool isCorrect = ValidateOrder();

        if (isCorrect)
        {
            feedbackText.text = "¡Correcto!";
            feedbackText.color = correctColor;
            HOSuperPrompt.Instance.OnAnswerCorrect();
        }
        else
        {
            feedbackText.text = "Incorrecto";
            feedbackText.color = incorrectColor;
            HOSuperPrompt.Instance.OnAnswerIncorrect();
        }
    }

    /// <summary>
    /// Valida que la selección del usuario coincida con el orden correcto.
    /// </summary>
    bool ValidateOrder()
    {
        for (int i = 0; i < selectionOrder.Count; i++)
        {
            int buttonIndex = selectionOrder[i];
            string userSelectedText = shuffledTexts[buttonIndex];
            string correctTextAtThisPosition = currentPrompt.correctOrder[i];

            if (userSelectedText != correctTextAtThisPosition)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Mezcla aleatoriamente las opciones disponibles.
    /// </summary>
    string[] ShuffleArray(string[] original)
    {
        string[] shuffled = (string[])original.Clone();
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        return shuffled;
    }

    /// <summary>
    /// Asigna texto a un botón que contiene un componente TextMeshProUGUI.
    /// </summary>
    void SetButtonText(Button button, string text)
    {
        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }
}