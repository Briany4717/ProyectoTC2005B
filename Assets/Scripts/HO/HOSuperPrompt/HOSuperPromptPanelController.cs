using UnityEngine;


/// Controla la visibilidad del panel principal de Super Prompt.

public class HOSuperPromptPanelController : MonoBehaviour
{
    public GameObject panel;

    public GameObject gameplayCanvas;

    
    /// Se suscribe a los eventos para mostrar u ocultar el panel.
    
    void Start()
    {
        HOSuperPrompt.Instance.OnPromptTriggered += ShowPanel;
        HOSuperPrompt.Instance.OnPromptClosed += HidePanel;

        panel.SetActive(false);
    }

    
    /// Se desuscribe de los eventos al destruirse.
    
    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnPromptTriggered -= ShowPanel;
            HOSuperPrompt.Instance.OnPromptClosed -= HidePanel;
        }
    }

    
    /// Activa el panel y oculta la interfaz de juego.
    
    void ShowPanel()
    {
        panel.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
    }

    
    /// Oculta el panel y vuelve a mostrar la interfaz de juego.
    
    void HidePanel()
    {
        panel.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);
    }
}