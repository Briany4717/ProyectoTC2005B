using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla las colisiones del jugador, específicamente permitiéndole bajar de plataformas.
/// </summary>
public class HOPlayerCol : MonoBehaviour
{
    private HOPlayerControl player;
    private float v; 
    private float timer;
    private bool startTimer;

    public Collider2D coll;
    
    /// <summary>
    /// Inicializa las variables de estado y la referencia del jugador.
    /// </summary>
    void Start()
    {
        player = GetComponentInParent<HOPlayerControl>();
        timer = 0.3f;
        startTimer = false;
    }

    /// <summary>
    /// Detecta la entrada hacia abajo para desactivar colisiones temporalmente.
    /// </summary>
    void Update()
    {
        v = 0f;
        if (Keyboard.current.downArrowKey.isPressed)  v = -1f;
        
        if (startTimer)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 0)
        {
            coll.enabled = true;
            timer = 0.3f;
            startTimer = false;
        }

        if (timer < -20)
        {
            timer = -18;
        }
    }

    /// <summary>
    /// Desactiva la colisión si el jugador presiona abajo al tocar el suelo.
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (v < 0)
            {
                coll.enabled = false;
                startTimer = true;
                v = 0f;
            }
        }
    }

    /// <summary>
    /// Mantiene la verificación de bajar plataformas mientras el jugador permanece en el suelo.
    /// </summary>
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (v < 0)
            {
                coll.enabled = false;
                startTimer = true;
                v= 0f;
            }
        }
    }
}