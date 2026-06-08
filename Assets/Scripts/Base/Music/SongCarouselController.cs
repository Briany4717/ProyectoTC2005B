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

    // Tu lista oficial usando tu modelo de datos
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

        // Simulación actualizada con las llaves exactas de tu modelo de datos ("id_cancion" y "nombre_cancion")
        string jsonSimulado = "[ {\"id_cancion\": \"1\", \"nombre_cancion\": \"Chicago - Michael\"}, {\"id_cancion\": \"2\", \"nombre_cancion\": \"Veridis Quo\"} ]";
        StartCoroutine(SimularEsperaApi(jsonSimulado));

        // Línea para cuando conectes tu backend real:
        // ApiManager.Instance.Get("usuarios/1/canciones/compras", OnSongsLoaded, OnApiError);
    }

    // BUG FIX #2: Declarado como IEnumerator plano (no IEnumerator<WaitForSeconds>).
    // Unity requiere IEnumerator base para StartCoroutine. El tipo genérico puede
    // causar advertencias o fallar en builds dependiendo de la versión de Unity.
    private IEnumerator SimularEsperaApi(string json)
    {
        yield return new WaitForSeconds(1f);
        OnSongsLoaded(json);
    }

    private void OnSongsLoaded(string jsonResponse)
    {
        // Envolvemos el array plano en un objeto para que JsonUtility lo pueda procesar
        string jsonModificado = "{ \"songs\": " + jsonResponse + " }";

        // Parseamos usando el Wrapper temporal adaptado
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

        // Usamos tu propiedad flecha 'title' que apunta a 'nombre_cancion'
        txtTitle.text = currentSong.title;

        btnLeft.interactable = (currentIndex > 0);
        btnRight.interactable = (currentIndex < unlockedSongs.Count - 1);

        // Usamos tu propiedad flecha 'id' que apunta a 'id_cancion'
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

        // BUG FIX #3: Siempre detenemos el preview y quitamos el mute,
        // independientemente de si se encontró el clip o no.
        // En la versión anterior, si selectedClip era null, el juego quedaba
        // silenciado permanentemente porque MuteGlobalAudio(false) nunca se llamaba.
        if (menuAudioSource != null) menuAudioSource.Stop();
        SongCatalog.Instance.MuteGlobalAudio(false);

        if (selectedClip != null)
        {
            SongCatalog.Instance.PlayGlobalMusic(selectedClip);
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

// ========================================================
// WRAPPER INTERNO (Asegúrate de que UserSong tenga [Serializable])
// ========================================================

[System.Serializable]
public class UserSongsWrapper
{
    public List<UserSong> songs;
}