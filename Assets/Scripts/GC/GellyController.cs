using UnityEngine;
using UnityEngine.InputSystem;


/// Controla los movimientos y acciones del jugador (Gelly).

[RequireComponent(typeof(Rigidbody2D))]
public class GellyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 10f;
    public LayerMask capaSuelo;
    public Transform puntoSuelo;
    public float radioSuelo = 0.2f;

    [Header("Computadora")]
    public float distanciaPC = 1.5f;
    public float posicionXPC = 0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool estaEnSuelo;
    private float ultimaDireccion = 0f;

    
    /// Obtiene referencias a componentes internos.
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    
    /// Revisa la entrada del jugador, físicas y la interacción con la computadora.
    
    void Update()
    {
        estaEnSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo);

        float inputX = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)  inputX = -1f;
        if (Keyboard.current.rightArrowKey.isPressed) inputX =  1f;

        rb.linearVelocity = new Vector2(inputX * velocidad, rb.linearVelocity.y);

        if (inputX != 0f) ultimaDireccion = inputX;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame && estaEnSuelo)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

        float distancia = Mathf.Abs(transform.position.x - posicionXPC);
        if (distancia <= distanciaPC && Keyboard.current.eKey.wasPressedThisFrame)
            SistemaCamaras.instancia.AbrirMenuPC();

        ActualizarAnimacion(inputX);
    }

    
    /// Cambia la animación según el estado de movimiento.
    
    void ActualizarAnimacion(float inputX)
    {
        if (!estaEnSuelo)
        {
            if (inputX < 0)      anim.Play("SaltarIzquierda");
            else if (inputX > 0) anim.Play("SaltarDerecha");
            else                 anim.Play("Saltar");
        }
        else if (inputX < 0)     anim.Play("CaminarIzquierda");
        else if (inputX > 0)     anim.Play("CaminarDerecha");
        else                     anim.Play("Idle");
    }

    
    /// Dibuja ayudas visuales en el editor para verificar el contacto con el suelo.
    
    void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }
    }
}