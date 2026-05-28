using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LEAudioManager : MonoBehaviour
{
    private static LEAudioManager instance;
    private AudioSource bgmAudioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Si ya existía una música sonando de la escena previa, aborta este clon
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ¡INMORTALIDAD!: La música flotará entre escenas

        bgmAudioSource = GetComponent<AudioSource>();
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = true;
    }

    public void ChangeMusicPitch(float newPitch)
    {
        if (bgmAudioSource != null) bgmAudioSource.pitch = newPitch;
    }
}
