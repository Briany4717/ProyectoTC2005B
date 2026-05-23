using UnityEngine;

public class LEStartSceneController : MonoBehaviour
{
    public GameObject playMenu;
    public GameObject startMenu;
    public LETutorialController tutorialController;

    public void StartTutorial()
    {
        playMenu.SetActive(false);
        startMenu.SetActive(false);
        tutorialController.StartTutorial();
    }

}
