using UnityEngine;
using System.Collections;

public class HOPlayerFallDetector : MonoBehaviour
{
    private float fallMargin = 1f;
    private Rigidbody2D rig;
    private bool isInvulnerable = false;
    private bool isFalling = false;

    public bool IsInvulnerable {get{return isInvulnerable;}}

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isFalling) 
        {
            return;
        }

        float threshold = HOScrollingCamera.Instance.BottomEdgeY - fallMargin;
        if (transform.position.y < threshold)
        {
            HandleFall();
        }
    }

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

    Vector3 findRespawnPos()
    {
        Vector3 cameraCenter = HOScrollingCamera.Instance.transform.position;
        return new Vector3(cameraCenter.x, cameraCenter.y, 0f);
    }

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