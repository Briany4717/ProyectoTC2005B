using UnityEngine;

public class GamesAudioController : MonoBehaviour
{
    private void Start()
    {
        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.PauseGlobalMusic();
            Debug.Log("Música global pausada porque este juego tiene su propia música.");
        }
    }

    private void OnDestroy()
    {
        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.ResumeGlobalMusic();
            Debug.Log("Música global reanudada al salir del juego.");
        }
    }
}