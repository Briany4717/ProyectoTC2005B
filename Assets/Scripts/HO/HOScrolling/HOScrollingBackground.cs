using UnityEngine;

public class HOScrollingBackground : MonoBehaviour
{
    public SpriteRenderer backgroundA;
    public SpriteRenderer backgroundB;
    private float spriteHeight;
    float margin = 1f;

    void Start()
    {
        spriteHeight = backgroundA.bounds.size.y;

        backgroundB.transform.position = new Vector3(
            backgroundA.transform.position.x,
            backgroundA.transform.position.y + spriteHeight,
            backgroundA.transform.position.z
        );
    }

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

    void subir(SpriteRenderer fondoParaSubir, SpriteRenderer referencia)
    {
        fondoParaSubir.transform.position = new Vector3(
            referencia.transform.position.x,
            referencia.transform.position.y + spriteHeight,
            referencia.transform.position.z
        );
    }
}