using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SongCarouselController : MonoBehaviour
{
    [Header("Componentes de UI")]
    public TextMeshProUGUI txtTitle;
    public Button btnLeft;
    public Button btnRight;
    public Button btnSelect;

    [Header("Audio Local de Previews")]
    public AudioSource menuAudioSource;

    private List<UserSong> unlockedSongs = new List<UserSong>();
    private int currentIndex = 0;

    private void Start()
    {
        btnLeft.onClick.AddListener(MoveLeft);
        btnRight.onClick.AddListener(MoveRight);
        btnSelect.onClick.AddListener(SelectSong);

        SetUiInteractable(false);
        txtTitle.text = "Cargando canciones...";

        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.MuteGlobalAudio(true);
        }

        // // Simulación actualizada con las llaves exactas de tu modelo de datos ("id_cancion" y "nombre_cancion")
        // string jsonSimulado = "[ {\"id_cancion\": \"1\", \"nombre_cancion\": \"Chicago - Michael\"}, {\"id_cancion\": \"2\", \"nombre_cancion\": \"Veridis Quo\"} ]";
        // StartCoroutine(SimularEsperaApi(jsonSimulado));

        // Línea para cuando conectes tu backend real:
        ApiManager.Instance.Get("/usuarios/1/canciones/compradas", OnSongsLoaded, OnApiError);
    }

    private IEnumerator SimularEsperaApi(string json)
    {
        yield return new WaitForSeconds(1f);
        OnSongsLoaded(json);
    }

    private void OnSongsLoaded(string jsonResponse)
    {
        string jsonModificado = "{ \"songs\": " + jsonResponse + " }";

        UserSongsWrapper wrapper = JsonUtility.FromJson<UserSongsWrapper>(jsonModificado);

        if (wrapper == null || wrapper.songs == null || wrapper.songs.Count == 0)
        {
            txtTitle.text = "No tienes canciones compradas";
            return;
        }

        unlockedSongs.Clear();
        unlockedSongs.AddRange(wrapper.songs);
        currentIndex = 0;

        SetUiInteractable(true);
        UpdateCarousel();
    }

    private void OnApiError(string errorMessage)
    {
        Debug.LogError($"Error al conectar con la API: {errorMessage}");
        txtTitle.text = "Error de conexión";
    }

    private void UpdateCarousel()
    {
        if (unlockedSongs.Count == 0) return;

        UserSong currentSong = unlockedSongs[currentIndex];

        txtTitle.text = currentSong.title;

        btnLeft.interactable = (currentIndex > 0);
        btnRight.interactable = (currentIndex < unlockedSongs.Count - 1);

        PlayAudioPreview(currentSong.id);
    }

    private void MoveLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateCarousel();
        }
    }

    private void MoveRight()
    {
        if (currentIndex < unlockedSongs.Count - 1)
        {
            currentIndex++;
            UpdateCarousel();
        }
    }

    public void SelectSong()
    {
        if (unlockedSongs.Count == 0) return;

        UserSong selected = unlockedSongs[currentIndex];
        Debug.Log($"Canción guardada para el juego: {selected.title}");

        AudioClip selectedClip = SongCatalog.Instance.GetClipById(selected.id);

        if (menuAudioSource != null) menuAudioSource.Stop();
        SongCatalog.Instance.MuteGlobalAudio(false);

        if (selectedClip != null)
        {
            SongCatalog.Instance.PlayGlobalMusic(selectedClip);
            GoToMenu();
        }
        else
        {
            Debug.LogWarning($"No se encontró clip para el ID '{selected.id}'. El audio global fue desmutado pero no se cambió la canción.");
        }
    }

    private void PlayAudioPreview(string songId)
    {
        if (menuAudioSource == null) return;

        AudioClip clip = SongCatalog.Instance.GetClipById(songId);
        if (clip != null)
        {
            menuAudioSource.Stop();
            menuAudioSource.clip = clip;
            menuAudioSource.Play();
        }
    }

    private void SetUiInteractable(bool state)
    {
        btnLeft.interactable = state;
        btnRight.interactable = state;
        btnSelect.interactable = state;
    }

    private void OnDestroy()
    {
        if (SongCatalog.Instance != null)
        {
            SongCatalog.Instance.MuteGlobalAudio(false);
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}


[System.Serializable]
public class UserSongsWrapper
{
    public List<UserSong> songs;
}