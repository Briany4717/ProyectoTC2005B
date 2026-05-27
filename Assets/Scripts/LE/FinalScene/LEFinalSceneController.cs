using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LEFinalSceneController : MonoBehaviour
{
    [Header("UI Background & Customization  ")]
    [SerializeField] private Image backgroundMainImage;
    [SerializeField] private Sprite winBackgroundSprite;
    [SerializeField] private Sprite loseBackgroundSprite;

    [Header("Gelly Character Animation")]
    [SerializeField] private Animator gellyAnimator;

    [Header("Telemetry Display (TMPro)")]
    [SerializeField] private TextMeshProUGUI repairedText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    void Start()
    {
        ProcessAndSaveMatchResults();
        DisplayMatchResultsUI();
    }

    private void ProcessAndSaveMatchResults()
    {
        var session = LEGameSessionData.Instance;

        // 1. CÁLCULO DE TELEMETRÍA FINAL
        int finalRepaired = session.repairedCount;
        int finalCoins = finalRepaired * 50;
        
        // Tiempo jugado = Duración total original - Segundos sobrantes en el reloj
        float timePlayedSeconds = session.totalMatchDuration - session.remainingTime;
        if (timePlayedSeconds < 0f) timePlayedSeconds = 0f;

        // 2. VOLCADO SEGURO A PLAYERPREFS
        PlayerPrefs.SetInt("AppliancesReparados", finalRepaired);
        PlayerPrefs.SetInt("MonedasObtenidas", finalCoins);
        PlayerPrefs.SetFloat("TiempoJugado", timePlayedSeconds);
        PlayerPrefs.Save(); // Forzamos la escritura en disco de inmediato

        // 3. CONFIGURACIÓN CINEMÁTICA EN UN SOLO FRAME
        if (session.isVictory)
        {
            if (backgroundMainImage != null && winBackgroundSprite != null)
                backgroundMainImage.sprite = winBackgroundSprite;

            if (gellyAnimator != null)
                gellyAnimator.SetTrigger("Win"); // Gatillo de Victoria
        }
        else
        {
            if (backgroundMainImage != null && loseBackgroundSprite != null)
                backgroundMainImage.sprite = loseBackgroundSprite;

            if (gellyAnimator != null)
                gellyAnimator.SetTrigger("Lose"); // Gatillo de Derrota
        }
    }

    private void DisplayMatchResultsUI()
    {
        var session = LEGameSessionData.Instance;
        
        int finalRepaired = session.repairedCount;
        int finalCoins = finalRepaired * 50;
        float timePlayedSeconds = session.totalMatchDuration - session.remainingTime;
        if (timePlayedSeconds < 0f) timePlayedSeconds = 0f;

        int minutes = Mathf.FloorToInt(timePlayedSeconds / 60f);
        int seconds = Mathf.FloorToInt(timePlayedSeconds % 60f);

        // Volcado de Rich Text ultra optimizado para la UI
        if (repairedText != null) repairedText.text = $"{finalRepaired} / 5";
        if (coinsText != null) coinsText.text = $"+{finalCoins}";
        if (timePlayedText != null) timePlayedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Vincula esta función pública al OnClick del botón de Reiniciar. (o^^)o
    /// </summary>
    public void OnClickRestartGameButton()
    {
        // Limpiamos la sesión estática por completo volviendo a setear el reloj a 300 segundos
        LEGameSessionData.Instance.ResetSession(300f); 

        // Cargamos la escena de la cinta (Donde los menús de inicio se re-armarán solos de forma limpia)
        SceneManager.LoadScene("LEConveyorScene");
    }
}
