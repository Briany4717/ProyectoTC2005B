using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

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

        int finalRepaired = session.repairedCount;
        int finalCoins = finalRepaired * 50;
        
        float timePlayedSeconds = session.totalMatchDuration - session.remainingTime;
        if (timePlayedSeconds < 0f) timePlayedSeconds = 0f;

        PlayerPrefs.SetInt("AppliancesReparados", finalRepaired);
        PlayerPrefs.SetInt("MonedasObtenidas", finalCoins);
        PlayerPrefs.SetFloat("TiempoJugado", timePlayedSeconds);
        PlayerPrefs.Save();

        if (session.isVictory)
        {
            if (backgroundMainImage != null && winBackgroundSprite != null)
                backgroundMainImage.sprite = winBackgroundSprite;

            if (gellyAnimator != null)
                gellyAnimator.SetTrigger("Win");
        }
        else
        {
            if (backgroundMainImage != null && loseBackgroundSprite != null)
                backgroundMainImage.sprite = loseBackgroundSprite;

            if (gellyAnimator != null)
                gellyAnimator.SetTrigger("Lose");
        }
    }

    private void DisplayMatchResultsUI()
    {
        var session = LEGameSessionData.Instance;
        
        int finalRepaired = session.repairedCount;
        int dificulty = PlayerPrefs.GetInt("LE_Minigames_Difficulty",1);
        int finalCoins = finalRepaired * 50 * (dificulty + 1);
        float timePlayedSeconds = session.totalMatchDuration - session.remainingTime;
        if (timePlayedSeconds < 0f) timePlayedSeconds = 0f;

        int minutes = Mathf.FloorToInt(timePlayedSeconds / 60f);
        int seconds = Mathf.FloorToInt(timePlayedSeconds % 60f);

        if (repairedText != null) repairedText.text = $"{finalRepaired} / 5";
        if (coinsText != null) coinsText.text = $"+{finalCoins}";
        if (timePlayedText != null) timePlayedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void OnClickRestartGameButton()
    {
        LEGameSessionData.Instance.ResetSession(PlayerPrefs.GetFloat("LE_Minigame_Duration", 330f)); 
        SceneManager.LoadScene("LEConveyorScene");
    }

    public void GoToMenu()
    {
        LEGameSessionData.Instance.ResetSession(PlayerPrefs.GetFloat("LE_Minigame_Duration", 330f)); 
        SceneManager.LoadScene("MenuScene");
    }
}
