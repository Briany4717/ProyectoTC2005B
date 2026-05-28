using System;
using Unity.VisualScripting;
using UnityEngine;


/// Permite al jugador interactuar con la estación del minijuego de control.

public class ControlPuzzleInteraction : MonoBehaviour, IInteractable
{
    public StationData station;
    [SerializeField] private GLControlManager GLControlManager;

    
    /// Inicia el minijuego de control al recibir la interacción.
    
    public void Interact()
    {
        Debug.Log("Estacion Control");
        GLControlManager.StartControlGame();
    }

    
    /// Evento disparado cuando el jugador deja de tocar la zona de interacción.
    
    public void OnNotTouchingPlayer()
    {
    }

    
    /// Evento disparado cuando el jugador entra en contacto con la zona de interacción.
    
    public void OnTouchingPlayer()
    {
    }

    
    /// Lógica ejecutada al completar la estación del minijuego de control.
    
    void completeStation()
    {
    }
}