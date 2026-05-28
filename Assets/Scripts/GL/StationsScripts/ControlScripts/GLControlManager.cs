using UnityEngine;


/// Administra el inicio y reinicio del minijuego de control.

public class GLControlManager : MonoBehaviour
{
    [SerializeField] private GLChecker glChecker;

    
    /// Abre el menú correspondiente al minijuego de control y reinicia su estado.
    
    public void StartControlGame()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Control);
        glChecker.RestartControlGame();
    }
}