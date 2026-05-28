using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Actualiza la barra de progreso de recolección de enemigos para el Super Prompt.
/// </summary>
public class HOSuperPromptUI : MonoBehaviour
{
    public Image barImage;

    public Sprite[] barStates;

    /// <summary>
    /// Inicializa la UI suscribiéndose a los eventos de progreso.
    /// </summary>
    void Start()
    {
        HOSuperPrompt.Instance.OnProgressChanged += UpdateBar;

        UpdateBar(HOSuperPrompt.Instance.EnemiesDefeated, HOSuperPrompt.Instance.EnemiesRequired);
    }

    /// <summary>
    /// Cancela la suscripción al destruirse.
    /// </summary>
    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnProgressChanged -= UpdateBar;
        }
    }

    /// <summary>
    /// Cambia el sprite de la barra dependiendo de la cantidad de enemigos derrotados.
    /// </summary>
    void UpdateBar(int current, int required)
    {
        int index = Mathf.Clamp(current, 0, barStates.Length - 1);
        barImage.sprite = barStates[index];
    }
}