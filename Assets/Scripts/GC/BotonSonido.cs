using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonSonido : MonoBehaviour
{
    [Tooltip("Desactiva el sonido en este botón específico")]
    public bool silenciado = false;

    void Start()
    {
        if (silenciado) return;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MusicController.instancia != null)
                MusicController.instancia.PlayClick();
        });
    }
}