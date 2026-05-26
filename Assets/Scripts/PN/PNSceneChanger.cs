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

    public void change(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
