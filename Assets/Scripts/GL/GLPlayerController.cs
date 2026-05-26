using System;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class GLPlayerController : MonoBehaviour
{
    // Declaración de variables
    [SerializeField] private float moveSpeed;
    public Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Animator animator;

    void Start()
    {
        // obtener los respectivos componentes del objeto
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadInput();
        UpdateAnimator();
    }


    // utilzamos FixedUpdate para tener un Update estable que no depende de los fps
    public void FixedUpdate()
    {
        // calculo de los vectores que determinan el moviento del jugador
        rig.linearVelocity = moveInput * moveSpeed;
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            return;
        }
        float x = (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
        float y = (Keyboard.current.upArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.downArrowKey.isPressed ? 1f : 0f);

        moveInput = new Vector2(x, y);
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

    }

    private void UpdateAnimator()
    {
        bool isWalking = moveInput.sqrMagnitude > 0f;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
    }

}
