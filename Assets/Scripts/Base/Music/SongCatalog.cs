using System;
using System.Collections.Generic;
using UnityEngine;

// Forzamos que el GameObject tenga un AudioSource acoplado
[RequireComponent(typeof(AudioSource))]
public class SongCatalog : MonoBehaviour
{
    public static SongCatalog Instance { get; private set; }

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

    private void Start()
    {
        if (!globalAudioSource.isPlaying)
        {
            AudioClip defaultClip = GetClipById("1");
            if (defaultClip != null)
            {
                PlayGlobalMusic(defaultClip);
            }
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