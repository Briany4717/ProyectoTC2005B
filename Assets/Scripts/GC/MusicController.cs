using UnityEngine;


/// Gestiona la reproducción global de la música y efectos de sonido en el juego.

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
    public AudioClip sfxDerrota;
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
    [Range(0f, 1f)] public float volumenDerrota  = 1f;

    private AudioSource musicaSource;
    private AudioSource sfxSource;

    public static MusicController instancia;

    
    /// Configura el singleton y los componentes de audio necesarios.

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

        volumenMusica       = PlayerPrefs.GetFloat("VolMusica", volumenMusica);
        volumenSFX          = PlayerPrefs.GetFloat("VolSFX",    volumenSFX);
        musicaSource.volume = volumenMusica;
        sfxSource.volume    = volumenSFX;
    }

    
    /// Inicia la reproducción de la música del juego si está asignada.

    void Start()
    {
        if (musicaJuego != null)
        {
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }

    
    /// Reinicia la música actual cargando los últimos valores de volumen guardados.

    public void ReiniciarMusica()
    {
        volumenMusica       = PlayerPrefs.GetFloat("VolMusica", volumenMusica);
        volumenSFX          = PlayerPrefs.GetFloat("VolSFX",    volumenSFX);
        musicaSource.volume = volumenMusica;
        sfxSource.volume    = volumenSFX;

        if (musicaJuego != null)
        {
            musicaSource.Stop();
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }

    
    /// Detiene la reproducción de la música actual.

    public void DetenerMusica() => musicaSource.Stop();

    
    /// Modifica y guarda el nivel de volumen global para la música.

    public void SetVolumenMusica(float valor)
    {
        volumenMusica = valor;
        if (musicaSource != null) musicaSource.volume = valor;
        PlayerPrefs.SetFloat("VolMusica", valor);
        PlayerPrefs.Save();
    }

    
    /// Modifica y guarda el nivel de volumen global para los efectos.

    public void SetVolumenSFX(float valor)
    {
        volumenSFX = valor;
        if (sfxSource != null) sfxSource.volume = valor;
        PlayerPrefs.SetFloat("VolSFX", valor);
        PlayerPrefs.Save();
    }

    
    /// Reproduce el efecto de sonido de clic.

    public void PlayClick()
    {
        if (sfxClick != null)
            sfxSource.PlayOneShot(sfxClick, volumenClick * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido de las cámaras.

    public void PlayCameras()
    {
        if (sfxCameras != null)
            sfxSource.PlayOneShot(sfxCameras, volumenCameras * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido al cerrar una puerta.

    public void PlayDoorSlam()
    {
        if (sfxDoorSlam != null)
            sfxSource.PlayOneShot(sfxDoorSlam, volumenDoorSlam * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido de descarga eléctrica.

    public void PlayElectricShock()
    {
        if (sfxElectricShock != null)
            sfxSource.PlayOneShot(sfxElectricShock, volumenElectric * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido de acierto o respuesta correcta.

    public void PlayRight()
    {
        if (sfxRight != null)
            sfxSource.PlayOneShot(sfxRight, volumenRight * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido de fallo o respuesta incorrecta.

    public void PlayWrong()
    {
        if (sfxWrong != null)
            sfxSource.PlayOneShot(sfxWrong, volumenWrong * volumenSFX);
    }

    
    /// Reproduce la alarma sonora de forma continua o puntual.

    public void PlayAlarm()
    {
        if (sfxAlarm != null)
            sfxSource.PlayOneShot(sfxAlarm, volumenAlarm * volumenSFX);
    }

    
    /// Detiene la alarma sonora (o cualquier efecto que se esté reproduciendo principal).

    public void StopAlarm() => sfxSource.Stop();

    
    /// Reproduce el efecto de sonido de victoria.

    public void PlayWin()
    {
        if (sfxWin != null)
            sfxSource.PlayOneShot(sfxWin, volumenWin * volumenSFX);
    }

    
    /// Reproduce el efecto de sonido de derrota.

    public void PlayDerrota()
    {
        if (sfxDerrota != null)
            sfxSource.PlayOneShot(sfxDerrota, volumenDerrota * volumenSFX);
    }

    
    /// Retorna el volumen actual de la música.

    public float GetVolumenMusica() => volumenMusica;

    
    /// Retorna el volumen actual de los efectos de sonido.

    public float GetVolumenSFX()    => volumenSFX;
}