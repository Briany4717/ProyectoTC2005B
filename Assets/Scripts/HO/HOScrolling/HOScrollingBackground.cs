using UnityEngine;

/// <summary>
/// Gestiona el desplazamiento infinito del fondo intercambiando dos sprites.
/// </summary>
public class HOScrollingBackground : MonoBehaviour
{
    public SpriteRenderer backgroundA;
    public SpriteRenderer backgroundB;
    private float spriteHeight;
    float margin = 1f;

    /// <summary>
    /// Calcula la altura del fondo y posiciona el segundo sprite justo arriba.
    /// </summary>
    void Start()
    {
        spriteHeight = backgroundA.bounds.size.y;

        backgroundB.transform.position = new Vector3(
            backgroundA.transform.position.x,
            backgroundA.transform.position.y + spriteHeight,
            backgroundA.transform.position.z
        );
    }

    /// <summary>
    /// Verifica si un fondo ha salido de la cámara para moverlo hacia arriba.
    /// </summary>
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

    /// <summary>
    /// Mueve un fondo por encima del otro para continuar el ciclo.
    /// </summary>
    void subir(SpriteRenderer fondoParaSubir, SpriteRenderer referencia)
    {
        fondoParaSubir.transform.position = new Vector3(
            referencia.transform.position.x,
            referencia.transform.position.y + spriteHeight,
            referencia.transform.position.z
        );
    }
}