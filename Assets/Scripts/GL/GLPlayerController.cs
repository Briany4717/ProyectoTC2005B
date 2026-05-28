using System;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;


/// Controla el movimiento y la animación del personaje del jugador.

public class GLPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Animator animator;

    
    /// Obtiene las referencias a los componentes de renderizado y animación.
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    
    /// Captura la entrada del jugador y actualiza las animaciones cada frame.
    
    void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    
    /// Aplica el movimiento físico basado en la entrada calculada de forma constante.
    
    public void FixedUpdate()
    {
        rig.linearVelocity = moveInput * moveSpeed;
    }

    
    /// Lee la entrada del teclado para determinar la dirección de movimiento.
    
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

    
    /// Ajusta los parámetros del Animator en base a la dirección y estado de movimiento.
    
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