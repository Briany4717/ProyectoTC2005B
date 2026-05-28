using UnityEngine;

/// <summary>
/// Controla la visibilidad del panel principal de Super Prompt.
/// </summary>
public class HOSuperPromptPanelController : MonoBehaviour
{
    public GameObject panel;

    public GameObject gameplayCanvas;

    /// <summary>
    /// Se suscribe a los eventos para mostrar u ocultar el panel.
    /// </summary>
    void Start()
    {
        HOSuperPrompt.Instance.OnPromptTriggered += ShowPanel;
        HOSuperPrompt.Instance.OnPromptClosed += HidePanel;

        panel.SetActive(false);
    }

    /// <summary>
    /// Se desuscribe de los eventos al destruirse.
    /// </summary>
    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnPromptTriggered -= ShowPanel;
            HOSuperPrompt.Instance.OnPromptClosed -= HidePanel;
        }
    }

    /// <summary>
    /// Activa el panel y oculta la interfaz de juego.
    /// </summary>
    void ShowPanel()
    {
        panel.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
    }

    /// <summary>
    /// Oculta el panel y vuelve a mostrar la interfaz de juego.
    /// </summary>
    void HidePanel()
    {
        panel.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);
    }
}