using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;

public class PNPlayerFallDetector : MonoBehaviour
{
    public float maxHeight;
    private Rigidbody2D rig;
    private bool isInvulnerable = false, isFalling = false;
    public Vector3 respawnPos;
    public PNGameController gameController;
    public bool IsInvulnerable {get{return isInvulnerable;}}
    public int minCoins = 5;
    SpriteRenderer sr;

    public void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        gameController = FindAnyObjectByType<PNGameController>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFalling) 
            return;

        if (transform.position.y < maxHeight)
            HandleFall();
    }

    void HandleFall()
    {
        isFalling = true;
        
        if (gameController == null)
            gameController = FindAnyObjectByType<PNGameController>();

        if (PlayerPrefs.GetInt("Coins") < minCoins)
        {
            PlayerPrefs.SetInt("Win", 0);
            if (gameController != null)
                gameController.gameOver();
        }
        else
        {
            PlayerPrefs.SetInt("Win", 1);
            gameController.SpendCoins();
            Respawn();   
        }
    }

    void Respawn()
    {
        transform.position = respawnPos;
        StartCoroutine(invulnerable());
        isFalling = false;
    }

    IEnumerator invulnerable()
    {
        isInvulnerable = true;
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