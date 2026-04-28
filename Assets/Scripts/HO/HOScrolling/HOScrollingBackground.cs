using UnityEngine;

public class HOScrollingBackground : MonoBehaviour
{
    public SpriteRenderer backgroundA;
    public SpriteRenderer backgroundB;

    public float margin = 1f;

    private float spriteHeight;

    void Start()
    {
        // Calcula la altura del sprite en unidades del mundo
        spriteHeight = backgroundA.bounds.size.y;

        // Asegura que B esté justo encima de A al inicio
        backgroundB.transform.position = new Vector3(
            backgroundA.transform.position.x,
            backgroundA.transform.position.y + spriteHeight,
            backgroundA.transform.position.z
        );
    }

    void Update()
    {
        float cameraBottom = HOScrollingCamera.Instance.bottomEdge;

        if (backgroundA.transform.position.y + spriteHeight / 2f < cameraBottom - margin)
        {
            subir(backgroundA, backgroundB);
        }

        // Lo mismo para B
        if (backgroundB.transform.position.y + spriteHeight / 2f < cameraBottom - margin)
        {
            subir(backgroundB, backgroundA);
        }
    }

    void subir(SpriteRenderer fondoParaSubir, SpriteRenderer referencia)
    {
        fondoParaSubir.transform.position = new Vector3(
            referencia.transform.position.x,
            referencia.transform.position.y + spriteHeight,
            referencia.transform.position.z
        );
    }
}