using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class HOErasablePlatform : MonoBehaviour
{
    [Header("Configuración de borrado")]
    [Tooltip("Duración del fade out al ser borrada")]
    public float fadeOutDuration = 0.8f;
    [Tooltip("Duración del fade in al reaparecer")]
    public float fadeInDuration = 0.5f;

    [Header("Reaparición")]
    [Tooltip("Si es true, la plataforma reaparece automáticamente después de X segundos")]
    public bool autoRespawn = true;
    [Tooltip("Tiempo en segundos antes de reaparecer")]
    public float respawnDelay = 5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isErased = false;

    public bool IsErased => isErased;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        HOPlatformRegistry.Register(this);
    }

    void OnDisable()
    {
        HOPlatformRegistry.Unregister(this);
    }

    /// <summary>
    /// Llama esto desde el borrador para iniciar el borrado.
    /// </summary>
    public void Erase()
    {
        if (isErased) return;
        StartCoroutine(EraseRoutine());
    }

    private IEnumerator EraseRoutine()
    {
        isErased = true;

        // Fade out progresivo
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeOutDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Desactiva el collider para que el jugador caiga
        platformCollider.enabled = false;
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        if (autoRespawn)
        {
            yield return new WaitForSeconds(respawnDelay);
            yield return RespawnRoutine(startColor);
        }
    }

    private IEnumerator RespawnRoutine(Color targetColor)
    {
        platformCollider.enabled = true;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, targetColor.a, elapsed / fadeInDuration);
            spriteRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        spriteRenderer.color = targetColor;
        isErased = false;
    }

    /// <summary>
    /// Por si quieres llamar la reaparición manualmente desde otro sistema.
    /// </summary>
    public void ForceRespawn()
    {
        if (!isErased) return;
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(Color.white));
    }
}