using UnityEngine;

/// <summary>
/// Administra la reproducción de música de fondo y efectos de sonido.
/// </summary>
public class GLSFXManager : MonoBehaviour
{
    [Header("------- Audio Source ---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip JeopardyFailure;
    public AudioClip JeopardyCorrect;
    public AudioClip Interaction;
    public AudioClip OrderComplete;
    public AudioClip SlideControlPuzzle;
    public AudioClip PaperSound;
    public AudioClip WallCrash;
    public AudioClip completeStation;

    public static GLSFXManager Instance;

    /// <summary>
    /// Configura el Singleton asegurando que solo haya un administrador de audio.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Reproduce la música de fondo al iniciar el componente.
    /// </summary>
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    /// <summary>
    /// Reproduce un efecto de sonido específico una sola vez.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}