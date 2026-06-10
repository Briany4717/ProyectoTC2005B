using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuSceneController : MonoBehaviour
{
    public RectTransform door;

    public float targetWidth = 1920f;
    public float duration = 1.0f;

    public AnimationCurve ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool _isAnimating = false;

    void Awake()
    {
        SetWidth(0f);
    }


    public void GoToMenu()        => LoadScene("MenuScene");
    public void GoToGlotones()    => LoadScene("GLStartScene");
    public void GoToHO()          => LoadScene("HOIntroScene");
    public void GoToPN()          => LoadScene("PNStartScene");
    public void GoToLE()          => LoadScene("LEConveyorScene");
    public void GoToGC()          => LoadScene("GCMenuPrincipal");
    public void GoToMusicConfig() => LoadScene("MusicScene");


    private void LoadScene(string sceneName)
    {
        if (_isAnimating) return;
        StartCoroutine(TransitionRoutine(sceneName));
        #if UNITY_WEBGL
        Input.ResetInputAxes(); 
        #endif
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        _isAnimating = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = ease.Evaluate(Mathf.Clamp01(elapsed / duration));
            SetWidth(Mathf.Lerp(0f, targetWidth, t));
            yield return null;
        }

        SetWidth(targetWidth);
        SceneManager.LoadScene(sceneName);
    }

    private void SetWidth(float w)
    {
        Vector2 size = door.sizeDelta;
        size.x = w;
        door.sizeDelta = size;
    }
}