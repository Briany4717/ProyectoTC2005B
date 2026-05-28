using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Música")]
    public AudioClip musicaJuego;
    [Range(0f, 1f)] public float volumenMusica = 0.5f;

    [Header("Efectos de sonido")]
    public AudioClip sfxAlarm;
    public AudioClip sfxCameras;
    public AudioClip sfxClick;
    public AudioClip sfxDoorSlam;
    public AudioClip sfxElectricShock;
    public AudioClip sfxRight;
    public AudioClip sfxWin;
    public AudioClip sfxWrong;
    public AudioClip sfxDerrota;          // ← NUEVO: arrastra tu clip de derrota
    [Range(0f, 1f)] public float volumenSFX = 1f;

    [Header("Volúmenes individuales")]
    [Range(0f, 1f)] public float volumenClick    = 0.7f;
    [Range(0f, 1f)] public float volumenCameras  = 0.8f;
    [Range(0f, 1f)] public float volumenDoorSlam = 1f;
    [Range(0f, 1f)] public float volumenElectric = 1f;
    [Range(0f, 1f)] public float volumenRight    = 1f;
    [Range(0f, 1f)] public float volumenWrong    = 1f;
    [Range(0f, 1f)] public float volumenAlarm    = 1f;
    [Range(0f, 1f)] public float volumenWin      = 1f;
    [Range(0f, 1f)] public float volumenDerrota  = 1f;  // ← NUEVO

    private AudioSource musicaSource;
    private AudioSource sfxSource;

    public static MusicController instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
        DontDestroyOnLoad(gameObject);

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            musicaSource = sources[0];
            sfxSource    = sources[1];
        }
        else if (sources.Length == 1)
        {
            musicaSource = sources[0];
            sfxSource    = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            musicaSource = gameObject.AddComponent<AudioSource>();
            sfxSource    = gameObject.AddComponent<AudioSource>();
        }

        musicaSource.loop        = true;
        musicaSource.playOnAwake = false;
        sfxSource.loop           = false;
        sfxSource.playOnAwake    = false;

        // ── Cargar valores guardados (claves unificadas) ──────────────
        volumenMusica      = PlayerPrefs.GetFloat("VolMusica", volumenMusica);
        volumenSFX         = PlayerPrefs.GetFloat("VolSFX",    volumenSFX);
        musicaSource.volume = volumenMusica;
        sfxSource.volume    = volumenSFX;
    }

    void Start()
    {
        if (musicaJuego != null)
        {
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }

    // ── Música ────────────────────────────────────────────────────────

    public void ReiniciarMusica()
    {
        // Recargar volúmenes guardados antes de reproducir
        volumenMusica      = PlayerPrefs.GetFloat("VolMusica", volumenMusica);
        volumenSFX         = PlayerPrefs.GetFloat("VolSFX",    volumenSFX);
        musicaSource.volume = volumenMusica;
        sfxSource.volume    = volumenSFX;

        if (musicaJuego != null)
        {
            musicaSource.Stop();
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }
    public void DetenerMusica() => musicaSource.Stop();

    public void SetVolumenMusica(float valor)
    {
        volumenMusica = valor;
        if (musicaSource != null) musicaSource.volume = valor;
        PlayerPrefs.SetFloat("VolMusica", valor);
        PlayerPrefs.Save();
    }

    public void SetVolumenSFX(float valor)
    {
        volumenSFX = valor;
        if (sfxSource != null) sfxSource.volume = valor;
        PlayerPrefs.SetFloat("VolSFX", valor);
        PlayerPrefs.Save();
    }

    // ── Efectos ───────────────────────────────────────────────────────

    public void PlayClick()
    {
        if (sfxClick != null)
            sfxSource.PlayOneShot(sfxClick, volumenClick * volumenSFX);
    }

    public void PlayCameras()
    {
        if (sfxCameras != null)
            sfxSource.PlayOneShot(sfxCameras, volumenCameras * volumenSFX);
    }

    public void PlayDoorSlam()
    {
        if (sfxDoorSlam != null)
            sfxSource.PlayOneShot(sfxDoorSlam, volumenDoorSlam * volumenSFX);
    }

    public void PlayElectricShock()
    {
        if (sfxElectricShock != null)
            sfxSource.PlayOneShot(sfxElectricShock, volumenElectric * volumenSFX);
    }

    public void PlayRight()
    {
        if (sfxRight != null)
            sfxSource.PlayOneShot(sfxRight, volumenRight * volumenSFX);
    }

    public void PlayWrong()
    {
        if (sfxWrong != null)
            sfxSource.PlayOneShot(sfxWrong, volumenWrong * volumenSFX);
    }

    public void PlayAlarm()
    {
        if (sfxAlarm != null)
            sfxSource.PlayOneShot(sfxAlarm, volumenAlarm * volumenSFX);
    }

    public void StopAlarm() => sfxSource.Stop();

    public void PlayWin()
    {
        if (sfxWin != null)
            sfxSource.PlayOneShot(sfxWin, volumenWin * volumenSFX);
    }

    public void PlayDerrota()
    {
        if (sfxDerrota != null)
            sfxSource.PlayOneShot(sfxDerrota, volumenDerrota * volumenSFX);
    }

    public float GetVolumenMusica() => volumenMusica;
    public float GetVolumenSFX()    => volumenSFX;
}