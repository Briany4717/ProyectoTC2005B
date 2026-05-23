using UnityEngine;

public class LEStartSceneController : MonoBehaviour
{
    public GameObject playMenu;
    public GameObject startMenu;
    public GameObject tutorialPanel;
    public LETutorialManager tutorialController;

    public void StartTutorial()
    {
        playMenu.SetActive(false);
        startMenu.SetActive(false);
        tutorialPanel.SetActive(true);
        tutorialController.StartTutorial();
    }

}
