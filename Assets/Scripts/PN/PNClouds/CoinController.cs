using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float seconds = 3, maxHeight = 10f;
    public string playerTag = "PNPlayer";
    public string buildingTag = "PNBuilding";
    //GameControl gameControl = FindObjectOfType<GameControl>();
    private Collider2D coincol; 

    void Awake()
    {
        coincol = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (transform.position.y <= maxHeight)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        float _seconds = col.gameObject.CompareTag(playerTag) ? 0 : seconds; 
        Destroy(gameObject, _seconds);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(playerTag))
        {
            Destroy(gameObject);
        }
        else if (col.gameObject.CompareTag(buildingTag))
        {
            Destroy(gameObject, seconds);
        }
    }
}
