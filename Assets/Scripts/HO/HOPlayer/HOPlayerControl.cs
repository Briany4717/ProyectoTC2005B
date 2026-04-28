using UnityEngine;
using UnityEngine.InputSystem;

public class HOPlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public Rigidbody2D rig;
    private float xInput;
    private bool jumpRequested;
    private int jumpCount = 0;
    private const int maxJumps = 2;

    private bool mirandoDerecha= true;


     void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            xInput = -1f;
            if(mirandoDerecha){girar();}
        } 
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            xInput = 1f;
            if(!mirandoDerecha){girar();}
        }
        if (Keyboard.current.upArrowKey.wasPressedThisFrame && jumpCount <maxJumps)
        {
            jumpRequested = true;
            jumpCount++;
        }
    }

    public void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(xInput * moveSpeed, rig.linearVelocity.y);
        
        if (jumpRequested)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        jumpCount = 0;
    }

    void girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y +180,0);
    }
}
