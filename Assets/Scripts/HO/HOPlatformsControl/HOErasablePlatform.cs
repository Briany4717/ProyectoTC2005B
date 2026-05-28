using System.Collections;
using UnityEngine;

/// <summary>
/// Gestiona el comportamiento de las plataformas que pueden ser borradas y su desvanecimiento.
/// </summary>
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

    /// <summary>
    /// Inicializa las referencias a los componentes.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Registra la plataforma en el gestor global al activarse.
    /// </summary>
    void OnEnable()
    {
        HOPlatformRegistry.Register(this);
    }

    /// <summary>
    /// Elimina la plataforma del registro al desactivarse.
    /// </summary>
    void OnDisable()
    {
        HOPlatformRegistry.Unregister(this);
    }

    /// <summary>
    /// Inicia el proceso de borrado de la plataforma.
    /// </summary>
    public void Erase()
    {
        if (isErased)
        {
            return;
        }
        StartCoroutine(eraseRoutine());
    }

    /// <summary>
    /// Corrutina para desvanecer la plataforma gradualmente y desactivar su colisión.
    /// </summary>
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
    
    /// <summary>
    /// Borra la plataforma automáticamente si sale de los límites de la pantalla.
    /// </summary>
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