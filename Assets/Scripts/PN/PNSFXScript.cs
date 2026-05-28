using UnityEngine;

public class PNSFXController : MonoBehaviour
{
    public static PNSFXController Instance { get; private set; }

    private AudioSource ASEffects, ASBackground;
    public AudioClip hunt, charge, coin, fah;
    public AudioClip[] stageMusic;

    public void StopMusic()
    {
        ASBackground.Stop();
    }

    public void PlayWinMusic()
    {
        ASBackground.clip = stageMusic[1];
        ASBackground.loop = false;
        ASBackground.Play();
    }

    public void PlayLoseMusic()
    {
        ASBackground.clip = stageMusic[2];
        ASBackground.loop = false;
        ASBackground.Play();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ASEffects = gameObject.AddComponent<AudioSource>();
            ASBackground = gameObject.AddComponent<AudioSource>();
            ASBackground.loop = true;
            ASBackground.volume = 0.5f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int index)
    {
        if (ASBackground.clip == stageMusic[index]) return;

        ASBackground.clip = stageMusic[index];
        ASBackground.Play();
    }

    public void coinSound()
    {
        ASEffects.PlayOneShot(coin);
    }

    public void chargeSound()
    {
        ASEffects.PlayOneShot(charge);
    }

    public void huntSound()
    {
        ASEffects.PlayOneShot(hunt);
    }

    public void pushSound()
    {
        ASEffects.PlayOneShot(fah);
    }
}
