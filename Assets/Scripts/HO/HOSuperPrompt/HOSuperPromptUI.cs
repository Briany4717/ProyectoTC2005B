using UnityEngine;
using UnityEngine.UI;


/// Actualiza la barra de progreso de recolección de enemigos para el Super Prompt.

public class HOSuperPromptUI : MonoBehaviour
{
    public Image barImage;

    public Sprite[] barStates;

    
    /// Inicializa la UI suscribiéndose a los eventos de progreso.
    
    void Start()
    {
        HOSuperPrompt.Instance.OnProgressChanged += UpdateBar;

        UpdateBar(HOSuperPrompt.Instance.EnemiesDefeated, HOSuperPrompt.Instance.EnemiesRequired);
    }

    
    /// Cancela la suscripción al destruirse.
    
    void OnDestroy()
    {
        if (HOSuperPrompt.Instance != null)
        {
            HOSuperPrompt.Instance.OnProgressChanged -= UpdateBar;
        }
    }

    
    /// Cambia el sprite de la barra dependiendo de la cantidad de enemigos derrotados.
    
    void UpdateBar(int current, int required)
    {
        int index = Mathf.Clamp(current, 0, barStates.Length - 1);
        barImage.sprite = barStates[index];
    }
}