using System.Collections;
using UnityEngine;

public class HOErasablePlatform : MonoBehaviour
{
    private float fadeOutDuration = 0.8f;
    // private float fadeInDuration = 0.5f;

    public bool autoRespawn = true;
    public float respawnDelay = 5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isErased = false;
    public bool IsErased {get{return isErased;}}

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    // registramos plataforma
    void OnEnable()
    {
        HOPlatformRegistry.Register(this);
    }

    // quitamos del registro
    void OnDisable()
    {
        HOPlatformRegistry.Unregister(this);
    }

    public void Erase()
    {
        if (isErased)
        {
            return;
        }
        StartCoroutine(eraseRoutine());
    }

    private IEnumerator eraseRoutine()
    {
        isErased = true;

        float elapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeOutDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        platformCollider.enabled = false;
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        //if (autoRespawn)
        //{
           // yield return new WaitForSeconds(respawnDelay);
            //yield return RespawnRoutine(startColor);
        //}
    }
    /*
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
    */
    /*
    public void ForceRespawn()
    {
        if (!isErased) return;
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(Color.white));
    }
    */
}