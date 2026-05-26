using UnityEngine;
using UnityEngine.InputSystem;

public class PNPlayerControl : MonoBehaviour
{
    public float moveSpeed, jumpForce;
    public Rigidbody2D rig;
    
    private float xInput;
    private bool jumpRequested, facingRight = true;
    private int jumpCount = 0;
    private const int maxJumps = 2;

    private PNHunt huntController;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        huntController = GetComponent<PNHunt>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleHuntInput();
    }

    void HandleMovementInput()
    {
        xInput = 0f;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            xInput = -1f;
            if (facingRight) Flip();
        } 
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed )
        {
            xInput = 1f;
            if (!facingRight) Flip();
        }

        if ((Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.isPressed)&& jumpCount < maxJumps)
        {
            jumpRequested = true;
            jumpCount++;
        }
    }

    void HandleHuntInput()
    {
        if (huntController == null) return;

        PNHunt.HunterState state = huntController.CurrentState;

        if (state == PNHunt.HunterState.Normal)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) huntController.EnterScanning();
        }
        else if (state == PNHunt.HunterState.Scanning)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) huntController.CycleTarget(1);

            if (Keyboard.current.enterKey.wasPressedThisFrame){
                huntController.EnterHunt();
                jumpCount = 0;
            }
            
            if (Keyboard.current.qKey.wasPressedThisFrame) huntController.ExitScanning();
        }
        else if (state == PNHunt.HunterState.Hunt)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame) huntController.CancelHunt();
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

    void Flip()
    {
        facingRight = !facingRight;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }
       
}