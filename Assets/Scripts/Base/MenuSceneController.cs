using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour
{


    public void GoToGlotones()
    {
        SceneManager.LoadScene("GLStartScene");
    }

    public void GoToHO()
    {
        SceneManager.LoadScene("HOIntroScene");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
