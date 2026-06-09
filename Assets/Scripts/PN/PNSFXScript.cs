using UnityEngine;

public class PNSFXController : MonoBehaviour
{
    public static PNSFXController Instance { get; private set; }

    private AudioSource ASEffects, ASBackground;
    public AudioClip hunt, charge, coin, fah;
    public AudioClip[] stageMusic;

    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float effectsVolume = 0.45f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ASEffects = gameObject.AddComponent<AudioSource>();
            ASBackground = gameObject.AddComponent<AudioSource>();
            ASBackground.loop = true;
            
            // Asignamos los volúmenes iniciales
            ASBackground.volume = musicVolume;
            ASEffects.volume = effectsVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (ASBackground != null) ASBackground.volume = musicVolume;
        if (ASEffects != null) ASEffects.volume = effectsVolume;
    }

    public void StopMusic()
    {
        ASBackground.Stop();
    }

    public void PlayWinMusic()
    {
        ASBackground.clip = stageMusic[1];
        ASBackground.loop = false;
        ASBackground.volume = musicVolume;
        ASBackground.Play();
    }

    public void PlayLoseMusic()
    {
        ASBackground.clip = stageMusic[2];
        ASBackground.loop = false;
        ASBackground.volume = musicVolume;
        ASBackground.Play();
    }

    public void PlayMusic(int index)
    {
        if (ASBackground.clip == stageMusic[index]) return;

        ASBackground.clip = stageMusic[index];
        ASBackground.volume = musicVolume; 
        ASBackground.Play();
    }

    public void coinSound()
    {
        ASEffects.PlayOneShot(coin, effectsVolume);
    }

    public void chargeSound()
    {
        ASEffects.PlayOneShot(charge, effectsVolume);
    }

    public void huntSound()
    {
        ASEffects.PlayOneShot(hunt, effectsVolume);
    }

    public void pushSound()
    {
        ASEffects.PlayOneShot(fah, effectsVolume);
    }
}