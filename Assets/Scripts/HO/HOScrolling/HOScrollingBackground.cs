using UnityEngine;


/// Gestiona el desplazamiento infinito del fondo intercambiando dos sprites.

public class HOScrollingBackground : MonoBehaviour
{
    public SpriteRenderer backgroundA;
    public SpriteRenderer backgroundB;
    private float spriteHeight;
    float margin = 1f;

    
    /// Calcula la altura del fondo y posiciona el segundo sprite justo arriba.
    
    void Start()
    {
        spriteHeight = backgroundA.bounds.size.y;

        backgroundB.transform.position = new Vector3(
            backgroundA.transform.position.x,
            backgroundA.transform.position.y + spriteHeight,
            backgroundA.transform.position.z
        );
    }

    
    /// Verifica si un fondo ha salido de la cámara para moverlo hacia arriba.
    
    void Update()
    {
        float camBottom = HOScrollingCamera.Instance.bottomEdge;

        if (backgroundA.transform.position.y + spriteHeight / 2f < camBottom - margin)
        {
            subir(backgroundA, backgroundB);
        }

        if (backgroundB.transform.position.y + spriteHeight / 2f < camBottom - margin)
        {
            subir(backgroundB, backgroundA);
        }
    }

    
    /// Mueve un fondo por encima del otro para continuar el ciclo.
    
    void subir(SpriteRenderer fondoParaSubir, SpriteRenderer referencia)
    {
        fondoParaSubir.transform.position = new Vector3(
            referencia.transform.position.x,
            referencia.transform.position.y + spriteHeight,
            referencia.transform.position.z
        );
    }
}