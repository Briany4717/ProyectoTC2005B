using UnityEngine;
using UnityEngine.InputSystem;

public class HOPlayerCol : MonoBehaviour
{
    private HOPlayerControl player;
    private float v; 
    private float timer;
    private bool startTimer;

    public Collider2D coll;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<HOPlayerControl>();
        timer = 0.3f;
        startTimer = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        v = 0f;
        if (Keyboard.current.downArrowKey.isPressed)  v = -1f;
        
        if (startTimer)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 0)
        {
            coll.enabled = true;
            timer = 0.3f;
            startTimer = false;
        }

        if (timer < -20)
        {
            timer = -18;
        }
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (v < 0)
            {
                coll.enabled = false;
                startTimer = true;
                v = 0f;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (v < 0)
            {
                coll.enabled = false;
                startTimer = true;
                v= 0f;
            }
        }
    }
}
