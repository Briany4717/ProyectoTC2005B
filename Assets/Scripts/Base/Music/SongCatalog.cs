using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Forzamos que el GameObject tenga un AudioSource acoplado
[RequireComponent(typeof(AudioSource))]
public class SongCatalog : MonoBehaviour
{
    public static SongCatalog Instance { get; private set; }

    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private string defaultSongId = "0";

    [Serializable]
    public struct SongItem
    {
        public string id;
        public AudioClip clip;
    }

    private AudioSource globalAudioSource;

    [Header("Biblioteca de Canciones del Juego")]
    public List<SongItem> database;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Configuramos el AudioSource Global de forma interna
            globalAudioSource = GetComponent<AudioSource>();
            globalAudioSource.loop = true;
            globalAudioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == loginSceneName)
        {
            PlayDefaultSong();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loginSceneName)
        {
            PlayDefaultSong();
        }
    }

    private void PlayDefaultSong()
    {
        AudioClip defaultClip = GetClipById(defaultSongId);
        if (defaultClip != null)
        {
            MuteGlobalAudio(false);
            PlayGlobalMusic(defaultClip);
        }
    }

    public AudioClip GetClipById(string id)
    {
        SongItem item = database.Find(s => s.id == id);
        if (item.clip != null)
        {
            return item.clip;
        }

        Debug.LogWarning($"No se encontró ningún AudioClip local para el ID: {id}");
        return null;
    }

    public void PlayGlobalMusic(AudioClip clip)
    {
        if (globalAudioSource.clip == clip && globalAudioSource.isPlaying) return;

        globalAudioSource.Stop();
        globalAudioSource.clip = clip;
        globalAudioSource.Play();
    }

    public void MuteGlobalAudio(bool state)
    {
        if (globalAudioSource != null)
        {
            globalAudioSource.mute = state;
        }
    }

    // Pausa por completo la musica global 
    public void PauseGlobalMusic()
    {
        if (globalAudioSource != null && globalAudioSource.isPlaying)
        {
            globalAudioSource.Pause();
        }
    }

    public void ResumeGlobalMusic()
    {
        if (globalAudioSource != null && !globalAudioSource.isPlaying)
        {
            globalAudioSource.UnPause();
        }
    }
}