using System;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla el movimiento y la animación del personaje del jugador.
/// </summary>
public class GLPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Animator animator;

    /// <summary>
    /// Obtiene las referencias a los componentes de renderizado y animación.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Captura la entrada del jugador y actualiza las animaciones cada frame.
    /// </summary>
    void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    /// <summary>
    /// Aplica el movimiento físico basado en la entrada calculada de forma constante.
    /// </summary>
    public void FixedUpdate()
    {
        rig.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Lee la entrada del teclado para determinar la dirección de movimiento.
    /// </summary>
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

    /// <summary>
    /// Ajusta los parámetros del Animator en base a la dirección y estado de movimiento.
    /// </summary>
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