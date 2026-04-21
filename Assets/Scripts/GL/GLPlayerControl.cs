using System;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    // Declaración de variables
    [SerializeField] private float moveSpeed;
    public Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private float xInput;
    private float yInput;
    private Animator animator;

    void Start()
    {
        // obtener los respectivos componentes del objeto
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void getPlayerMovementInput()
    {
        // declaramos una variable que nos ayudara para determinar a donde se movera nuestro personaje
        xInput = 0f;
        yInput = 0f;
        animator.SetBool("isWalking", false);

        if (Keyboard.current.aKey.isPressed)
        {
            xInput = -1f;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", xInput);
            animator.SetFloat("InputY", yInput);
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            xInput = 1f;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", xInput);
            animator.SetFloat("InputY", yInput);
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            yInput = 1f;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", xInput);
            animator.SetFloat("InputY", yInput);
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            yInput = -1f;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", xInput);
            animator.SetFloat("InputY", yInput);
        }
    }

    void Update()
    {
        getPlayerMovementInput();
    }


    // utilzamos FixedUpdate para tener un Update estable que no depende de los fps
    public void FixedUpdate()
    {
        // calculo de los vectores que determinan el moviento del jugador
        rig.linearVelocity = new Vector2(xInput * moveSpeed, yInput * moveSpeed);
    }
}
