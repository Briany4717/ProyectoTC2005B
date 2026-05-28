using System.Collections;
using UnityEngine;

public class PNStormBehaviour : MonoBehaviour
{
    public float speed = 3f, pushForce = 5f, xLimit =  -11F;
    public string playerTag = "PNPlayer";
    Rigidbody2D rb;

    void Awake()
    {
    }

    void Update()
    {
         if(transform.position.x <= xLimit)
            Destroy(gameObject);
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(playerTag))
        {
            PNPlayerControl player = col.GetComponent<PNPlayerControl>();
            if (player != null)
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                player.ApplyKnockback(dir * pushForce);
            }

            if (PNSFXController.Instance != null)  PNSFXController.Instance.pushSound();
        }
    }
}