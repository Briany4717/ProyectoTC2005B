using UnityEngine;


/// Administra la reproducción de música de fondo y efectos de sonido.

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

    
    /// Configura el Singleton asegurando que solo haya un administrador de audio.
    
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

    
    /// Reproduce la música de fondo al iniciar el componente.
    
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    
    /// Reproduce un efecto de sonido específico una sola vez.
    
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}