using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Permite al jugador interactuar con la estación del minijuego de control.
/// </summary>
public class ControlPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private GLControlManager GLControlManager;

    /// <summary>
    /// Inicia el minijuego de control al recibir la interacción.
    /// </summary>
    public void Interact()
    {
        Debug.Log("Estacion Control");
        GLControlManager.StartControlGame();
    }

    /// <summary>
    /// Evento disparado cuando el jugador deja de tocar la zona de interacción.
    /// </summary>
    public void OnNotTouchingPlayer()
    {
    }

    /// <summary>
    /// Evento disparado cuando el jugador entra en contacto con la zona de interacción.
    /// </summary>
    public void OnTouchingPlayer()
    {
    }

    /// <summary>
    /// Lógica ejecutada al completar la estación del minijuego de control.
    /// </summary>
    void completeStation()
    {
    }
}