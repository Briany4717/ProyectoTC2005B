using UnityEngine;

public class PNSFXController : MonoBehaviour
{
    public static PNSFXController Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] stateMusic; // 0=Prompt, 1=Golden, 2=Storm

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayMusic(int state)
    {
        if (musicSource == null) return;
        if (stateMusic == null || state < 0 || state >= stateMusic.Length) return;
        if (musicSource.clip == stateMusic[state] && musicSource.isPlaying) return;

        musicSource.clip = stateMusic[state];
        musicSource.loop = true;
        musicSource.Play();
    }
}
