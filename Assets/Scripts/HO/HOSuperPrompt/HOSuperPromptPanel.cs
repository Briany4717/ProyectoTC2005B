using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class HOSuperPromptPanel : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI feedbackText;

    public Button[] promptButtons;
    public Button confirmButton;

    [Header("Colores de feedback")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color neutralColor = Color.white;

    // Estado de la ronda actual
    private RespuestaOrdenamiento[] currentAnswers; // respuestas correctas (con su 'orden')
    private RespuestaOrdenamiento[] shuffledAnswers; // mismas respuestas, barajadas para mostrar
    private List<int> selectionOrder; // índices de botones en el orden que el usuario eligió

    void Awake()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            int index = i;
            promptButtons[i].onClick.AddListener(() => OnPromptButtonClicked(index));
        }
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    void OnEnable()
    {
        LoadRoundFromApi();
    }

    /// Pide una pregunta al API y, con su id, pide las respuestas.
    void LoadRoundFromApi()
    {
        selectionOrder = new List<int>();
        feedbackText.text = "Cargando...";
        feedbackText.color = neutralColor;
        SetButtonsInteractable(false);

        ApiManager.Instance.Get(
            "preguntasOrdenamiento",
            onSuccess: json =>
            {
                PreguntaOrdenamiento[] preguntas = JsonHelper.FromJsonArray<PreguntaOrdenamiento>(json);

                if (preguntas == null || preguntas.Length == 0)
                {
                    OnLoadError("No hay preguntas disponibles");
                    return;
                }

                // Elige una pregunta al azar
                PreguntaOrdenamiento pregunta = preguntas[Random.Range(0, preguntas.Length)];
                instructionText.text = pregunta.contenido;
                descriptionText.text = "";

                LoadAnswers(pregunta.id_pregunta);
            },
            onError: err => OnLoadError($"Error al cargar pregunta: {err}")
        );
    }

    /// Segunda llamada: trae las respuestas de la pregunta elegida.
    void LoadAnswers(int idPregunta)
    {
        ApiManager.Instance.Get(
            $"respuestasDePregunta/{idPregunta}",
            onSuccess: json =>
            {
                currentAnswers = JsonHelper.FromJsonArray<RespuestaOrdenamiento>(json);

                if (currentAnswers == null || currentAnswers.Length == 0)
                {
                    OnLoadError("La pregunta no tiene respuestas");
                    return;
                }

                // Ordena por 'orden' para tener la secuencia correcta de referencia
                currentAnswers = currentAnswers.OrderBy(a => a.orden).ToArray();

                // Baraja una copia para mostrar en los botones
                shuffledAnswers = ShuffleArray(currentAnswers);

                feedbackText.text = "";
                SetupButtons();
                SetButtonsInteractable(true);
            },
            onError: err => OnLoadError($"Error al cargar respuestas: {err}")
        );
    }

    void OnLoadError(string message)
    {
        feedbackText.text = message;
        feedbackText.color = incorrectColor;
        Debug.LogError($"[HOSuperPromptPanel] {message}");
    }

    /// Pone el texto de cada respuesta barajada en su botón.
    void SetupButtons()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            if (i < shuffledAnswers.Length)
            {
                SetButtonText(promptButtons[i], shuffledAnswers[i].contenido);
                promptButtons[i].gameObject.SetActive(true);
            }
            else
            {
                promptButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnPromptButtonClicked(int buttonIndex)
    {
        if (selectionOrder.Contains(buttonIndex))
            selectionOrder.Remove(buttonIndex);
        else
            selectionOrder.Add(buttonIndex);

        UpdateButtonLabels();
    }

    void UpdateButtonLabels()
    {
        for (int i = 0; i < promptButtons.Length; i++)
        {
            if (i >= shuffledAnswers.Length) continue;

            string baseText = shuffledAnswers[i].contenido;
            int orderPosition = selectionOrder.IndexOf(i);

            SetButtonText(promptButtons[i],
                orderPosition >= 0 ? $"[{orderPosition + 1}] {baseText}" : baseText);
        }
    }

    void OnConfirmClicked()
    {
        if (shuffledAnswers == null) return; // aún cargando

        if (selectionOrder.Count < currentAnswers.Length)
        {
            feedbackText.text = "Selecciona todas las partes en orden";
            feedbackText.color = incorrectColor;
            return;
        }

        if (ValidateOrder())
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

    /// Valida comparando el 'orden' del API, no strings.
    bool ValidateOrder()
    {
        for (int i = 0; i < selectionOrder.Count; i++)
        {
            int buttonIndex = selectionOrder[i];
            RespuestaOrdenamiento selected = shuffledAnswers[buttonIndex];

            // La posición i (0-based) debe corresponder a orden i+1 (1-based)
            if (selected.orden != i + 1)
                return false;
        }
        return true;
    }

    RespuestaOrdenamiento[] ShuffleArray(RespuestaOrdenamiento[] original)
    {
        RespuestaOrdenamiento[] shuffled = (RespuestaOrdenamiento[])original.Clone();
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        return shuffled;
    }

    void SetButtonsInteractable(bool value)
    {
        foreach (var b in promptButtons) b.interactable = value;
        confirmButton.interactable = value;
    }

    void SetButtonText(Button button, string text)
    {
        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }
}