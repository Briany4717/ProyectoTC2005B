using System.Collections;
using UnityEngine;

public class HOErasablePlatform : MonoBehaviour
{
    public bool eraseOffScreen = true;
    public float offscreenMargin = 2f;
    public bool destroyAfterErase = false;


    private float fadeOutDuration = 0.8f;
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
        if (destroyAfterErase)
        {
            Destroy(gameObject);
        }

    }
    void Update()
    {
        if (isErased) return;
        if (!eraseOffScreen) return;
        if (HOScrollingCamera.Instance == null) return;

        float threshold = HOScrollingCamera.Instance.bottomEdge - offscreenMargin;
        if (transform.position.y < threshold)
        {
            destroyAfterErase = true;
            Erase();
        }
    }
}