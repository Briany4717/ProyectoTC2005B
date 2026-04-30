using UnityEngine;

public class HOSuperPromptPanelController : MonoBehaviour
{
    public GameObject panel;

    public GameObject gameplayCanvas;

    void Start()
    {
        HOSuperPrompt.Instance.OnPromptTriggered += ShowPanel;
        HOSuperPrompt.Instance.OnPromptClosed += HidePanel;

        panel.SetActive(false);
    }

    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnPromptTriggered -= ShowPanel;
            HOSuperPrompt.Instance.OnPromptClosed -= HidePanel;
        }
    }

    void ShowPanel()
    {
        panel.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
    }

    void HidePanel()
    {
        panel.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);
    }
}