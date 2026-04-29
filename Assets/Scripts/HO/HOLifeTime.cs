using UnityEngine;

public class HOLifeTime : MonoBehaviour
{
    public float tiempoDeVida;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}
