using UnityEngine;

public class HOSound : MonoBehaviour
{
    public AudioClip sound;

    public void Start()
    {
        AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position, 0.5f);
    }
}
