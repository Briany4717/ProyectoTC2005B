using UnityEngine;
using UnityEngine.SceneManagement;

public class PNSceneChanger : MonoBehaviour
{
    void Start()
    {
    }   

    
    void Update()
    {
    }

    public void startGame()
    {
        SceneManager.LoadScene("PNMainGame");
    }

    public void change(string scene)
    {
        PNSFXController.Instance.StopMusic();
        SceneManager.LoadScene(scene);
    }
}
