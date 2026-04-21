using System;
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

    void Start()
    {
        // obtener los respectivos componentes del objeto
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    void Update()
    {
        // declaramos una variable que nos ayudara para determinar a donde se movera nuestro personaje
        xInput = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            // si apretamos la flecha izquierda asignar -1
            xInput = -1f;
            // rotar nuestro personaje en el eje x, para dar la ilusion de caminar a la izquierda
            // spriteRenderer.flipX = true;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            // si apretamos la flecha izquierda asignar -1
            xInput = 1f;
            // el personaje se mantiene en su posicion
            // spriteRenderer.flipX = false;
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            yInput = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            yInput = -1f;
        }
    }


    // utilzamos FixedUpdate para tener un Update estable que no depende de los fps
    public void FixedUpdate()
    {
        // calculo de los vectores que determinan el moviento del jugador
        rig.linearVelocity = new Vector2(xInput * moveSpeed, yInput * moveSpeed);
    }
}
