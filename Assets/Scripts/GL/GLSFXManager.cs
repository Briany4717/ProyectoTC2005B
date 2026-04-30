using UnityEngine;

public class GLSFXManager : MonoBehaviour
{
    // Declaracion de variables
    [Header("------- Audio Source ---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip JeopardyFailure;

    // creamos una instancia global para facilitar el uso de nuestro Manager a traves de los scripts
    public static GLSFXManager Instance;

    private void Awake()
    {
        // verificamos que no exista y la creamos 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // si ya existe una destruimos la actual (para no duplicar instancias)
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // asignamos el clip de background a music y lo reproducimos
        musicSource.clip = background;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        // reproducimos una vez el clip que le pasemo
        SFXSource.PlayOneShot(clip);
    }
}
