using UnityEngine;
using UnityEngine.InputSystem;


/// Controla las colisiones del jugador, específicamente permitiéndole bajar de plataformas.

public class HOPlayerCol : MonoBehaviour
{
    private HOPlayerControl player;
    private float v; 
    private float timer;
    private bool startTimer;

    public Collider2D coll;
    
    
    /// Inicializa las variables de estado y la referencia del jugador.
    
    void Start()
    {
        player = GetComponentInParent<HOPlayerControl>();
        timer = 0.3f;
        startTimer = false;
    }

    
    /// Detecta la entrada hacia abajo para desactivar colisiones temporalmente.
    
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

    
    /// Desactiva la colisión si el jugador presiona abajo al tocar el suelo.
    
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

    
    /// Mantiene la verificación de bajar plataformas mientras el jugador permanece en el suelo.
    
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