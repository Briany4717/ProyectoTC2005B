using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla el movimiento principal, salto y giro del jugador.
/// </summary>
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

    /// <summary>
    /// Obtiene el componente Rigidbody2D.
    /// </summary>
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Captura la entrada del usuario para movimiento y otras acciones.
    /// </summary>
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
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            HOSuperPrompt.Instance.OnAnswerCorrect();
        }
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            HOSuperPrompt.Instance.OnAnswerIncorrect();
        }
    }

    /// <summary>
    /// Aplica el movimiento y el salto usando las físicas.
    /// </summary>
    public void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(xInput * moveSpeed, rig.linearVelocity.y);
        
        if (jumpRequested)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    /// <summary>
    /// Reinicia el contador de saltos al tocar una superficie.
    /// </summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        jumpCount = 0;
    }

    /// <summary>
    /// Invierte la dirección visual del jugador.
    /// </summary>
    void girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y +180,0);
    }
}