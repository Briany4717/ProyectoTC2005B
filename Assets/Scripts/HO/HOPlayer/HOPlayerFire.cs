using UnityEngine;
using UnityEngine.InputSystem;

public class HOPlayerFire : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject proyectil;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shot();
        }
    }

    void shot()
    {
        Instantiate(proyectil, controladorDisparo.position, controladorDisparo.rotation);
    }
}