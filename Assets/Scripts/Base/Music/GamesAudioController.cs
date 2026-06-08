using UnityEngine;

public class GamesAudioController : MonoBehaviour
{
    private void Start()
    {
        // En cuanto el nivel empieza, le decimos al catálogo global que se pause.
        // La canción queda congelada exactamente en el segundo en que iba.
        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.PauseGlobalMusic();
            Debug.Log("Música global pausada porque este juego tiene su propia música.");
        }
    }

    private void OnDestroy()
    {
        // Cuando el jugador sale del nivel (vuelve al menú principal o menú de selección),
        // intentamos reanudar con UnPause().
        //
        // NOTA (Bug 4 — comportamiento documentado, no un error):
        // Si el jugador seleccionó una canción nueva en el carrusel DESPUÉS de que la
        // música fue pausada aquí, PlayGlobalMusic() ya habrá llamado Stop() + Play(),
        // por lo que el AudioSource estará en estado "playing", no "paused".
        // En ese caso, ResumeGlobalMusic() detecta isPlaying == true y no hace nada,
        // lo cual es el comportamiento correcto — la música ya está sonando.
        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.ResumeGlobalMusic();
            Debug.Log("Música global reanudada al salir del juego.");
        }
    }
}