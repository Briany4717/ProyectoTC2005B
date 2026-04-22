using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class HOPlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public Rigidbody2D rig;
    private float xInput;
    private bool jumpRequested;
    private bool isGrounded = true;

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
        } else if (Keyboard.current.rightArrowKey.isPressed)
        {
            xInput = 1f;
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {
            jumpRequested = true;
            isGrounded = false;
        }

    }

    public void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(xInput * moveSpeed, rig.linearVelocity.y);
        if (jumpRequested)
        {
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpRequested = false;
        }
        if (Math.Abs(rig.linearVelocity.y) < 0.01f)
        {
            isGrounded = true;
        }
    }
}
