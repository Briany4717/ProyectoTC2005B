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
        public string id;          // Coincide con el ID de la API
        public AudioClip clip;     // El archivo .mp3/.wav en tus Assets
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
        // BUG FIX #1: Revisamos isPlaying en lugar de solo clip == null.
        // Si el AudioSource tiene un clip asignado en el Inspector pero no está
        // sonando (playOnAwake = false), la condición anterior nunca arrancaba la música.
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

    // Permite al carrusel apagar y encender la música de fondo de manera limpia.
    // Nota: usa Mute (no Pause) intencionalmente para que la pista siga avanzando
    // en el tiempo mientras el carrusel está abierto.
    public void MuteGlobalAudio(bool state)
    {
        if (globalAudioSource != null)
        {
            globalAudioSource.mute = state;
        }
    }

    // Pausa por completo la música global (útil para cuando entras a un nivel con música propia)
    public void PauseGlobalMusic()
    {
        if (globalAudioSource != null && globalAudioSource.isPlaying)
        {
            globalAudioSource.Pause();
        }
    }

    // Reanuda la música que ya estaba pausada (útil para cuando sales del nivel al menú).
    // Nota: si se llamó PlayGlobalMusic() después de pausar, el AudioSource ya estará en Play()
    // y UnPause() no tendrá efecto — esto es el comportamiento correcto y esperado.
    public void ResumeGlobalMusic()
    {
        if (globalAudioSource != null && !globalAudioSource.isPlaying)
        {
            globalAudioSource.UnPause();
        }
    }
}