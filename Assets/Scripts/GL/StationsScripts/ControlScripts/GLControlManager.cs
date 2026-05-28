using UnityEngine;

/// <summary>
/// Administra el inicio y reinicio del minijuego de control.
/// </summary>
public class GLControlManager : MonoBehaviour
{
    [SerializeField] private GLChecker glChecker;

    /// <summary>
    /// Abre el menú correspondiente al minijuego de control y reinicia su estado.
    /// </summary>
    public void StartControlGame()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Control);
        glChecker.RestartControlGame();
    }
}