using UnityEngine;
using System.Collections;


/// Detecta si el jugador cae fuera de los límites de la pantalla.

public class HOPlayerFallDetector : MonoBehaviour
{
    private float fallMargin = 1f;
    private Rigidbody2D rig;
    private bool isInvulnerable = false;
    private bool isFalling = false;

    public bool IsInvulnerable {get{return isInvulnerable;}}

    
    /// Obtiene el componente Rigidbody2D al iniciar.
    
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    
    /// Comprueba si el jugador ha caído por debajo del límite de la cámara.
    
    void Update()
    {
        if (isFalling) 
        {
            return;
        }

        float threshold = HOScrollingCamera.Instance.bottomEdge - fallMargin;
        if (transform.position.y < threshold)
        {
            HandleFall();
        }
    }

    
    /// Resta vida al jugador cuando cae.
    
    void HandleFall()
    {
        isFalling = true;

        HOPlayerLives lives = GetComponent<HOPlayerLives>();
        if (lives != null)
        {
            lives.TomarDanio(1);
        }

        if (lives.cantidadDeVida > 0)
        {
            Respawn();
        }
    }

    
    /// Reposiciona al jugador e inicia su invulnerabilidad.
    
    void Respawn()
    {
        Vector3 respawnPos = findRespawnPos();
        transform.position = respawnPos;

        if (rig != null)
        {
            rig.linearVelocity = Vector2.zero;
        }

        StartCoroutine(invulnerable());
        isFalling = false;
    }

    
    /// Encuentra una posición de reaparición basada en la cámara.
    
    Vector3 findRespawnPos()
    {
        Vector3 cameraCenter = HOScrollingCamera.Instance.transform.position;
        return new Vector3(cameraCenter.x, cameraCenter.y, 0f);
    }

    
    /// Corrutina para otorgar invulnerabilidad temporal después de reaparecer.
    
    IEnumerator invulnerable()
    {
        isInvulnerable = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float parpadeo = 0.1f;
        float elapsed = 0f;
        float tiempoInvulnerable = 1.5f;

        while (elapsed < tiempoInvulnerable)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(parpadeo);
            elapsed += parpadeo;
        }

        if (sr != null) 
        {
            sr.enabled = true;
        }
        isInvulnerable = false;
    }
}