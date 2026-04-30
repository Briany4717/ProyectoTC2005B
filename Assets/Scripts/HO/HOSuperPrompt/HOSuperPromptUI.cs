using UnityEngine;
using UnityEngine.UI;

public class HOSuperPromptUI : MonoBehaviour
{
    public Image barImage;

    public Sprite[] barStates;

    void Start()
    {
        // subs al evento del manager
        HOSuperPrompt.Instance.OnProgressChanged += UpdateBar;

        UpdateBar(HOSuperPrompt.Instance.EnemiesDefeated, HOSuperPrompt.Instance.EnemiesRequired);
    }

    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnProgressChanged -= UpdateBar;
        }
    }

    void UpdateBar(int current, int required)
    {
        int index = Mathf.Clamp(current, 0, barStates.Length - 1);
        barImage.sprite = barStates[index];
    }
}