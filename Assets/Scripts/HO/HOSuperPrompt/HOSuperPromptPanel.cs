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


/// Gestiona la interfaz y la lógica de interacción del panel del Super Prompt.

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

    
    /// Configura los listeners de los botones al inicializar.
    
    void Awake()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            int index = i; 
            promptButtons[i].onClick.AddListener(() => OnPromptButtonClicked(index));
        }

        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    
    /// Prepara una nueva ronda cuando el panel se activa.
    
    void OnEnable()
    {
        SetupRound();
    }

    
    /// Configura los textos y mezcla las opciones de la pregunta actual.
    
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

    
    /// Maneja la selección y deselección de opciones por parte del usuario.
    
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

    
    /// Actualiza los textos de los botones para mostrar el orden de selección.
    
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

    
    /// Verifica si el orden seleccionado es el correcto al confirmar.
    
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

    
    /// Valida que la selección del usuario coincida con el orden correcto.
    
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

    
    /// Mezcla aleatoriamente las opciones disponibles.
    
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

    
    /// Asigna texto a un botón que contiene un componente TextMeshProUGUI.
    
    void SetButtonText(Button button, string text)
    {
        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }
}