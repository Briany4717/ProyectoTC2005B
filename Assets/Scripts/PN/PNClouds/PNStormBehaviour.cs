using System.Collections;
using UnityEngine;

public class PNStormBehaviour : MonoBehaviour
{
    public float speed = 3f, pushForce = 5f, xLimit =  -11F;
    public string playerTag = "PNPlayer";
    Rigidbody2D rb;
    private Collider2D cloudcol; 

    void Awake()
    {
        cloudcol = GetComponent<Collider2D>();
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
            Debug.Log("aa");
            rb = col.attachedRigidbody;
            Vector2 dir = (col.transform.position - transform.position).normalized;
            rb.AddForce(dir * pushForce, ForceMode2D.Impulse);
        }
        
    }
}