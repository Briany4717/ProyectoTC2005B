// CoverDisplay.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CoverDisplay : MonoBehaviour
{
    [Header("UI")]
    public Image coverImage;

    private Coroutine downloadRoutine;

    // El carrusel llama esto pasándole la url_imagen de la canción actual.
    public void ShowCover(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        if (downloadRoutine != null) StopCoroutine(downloadRoutine);
        downloadRoutine = StartCoroutine(DownloadCover(imageUrl));
    }

    private IEnumerator DownloadCover(string url)
    {
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error al bajar portada: {req.error}");
            yield break;
        }

        Texture2D tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
        coverImage.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }
}