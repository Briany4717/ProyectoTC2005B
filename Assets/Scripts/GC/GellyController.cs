using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

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

        // Abrir cámaras al acercarse a la PC
        float distancia = Mathf.Abs(transform.position.x - posicionXPC);
        if (distancia <= distanciaPC && Keyboard.current.eKey.wasPressedThisFrame)
            SistemaCamaras.instancia.AbrirMenuPC();

        ActualizarAnimacion(inputX);
    }

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

    void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }
    }
}