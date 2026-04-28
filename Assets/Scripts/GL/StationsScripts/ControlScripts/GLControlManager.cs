using UnityEngine;


public class GLControlManager : MonoBehaviour
{
    [SerializeField] private GLChecker glChecker;

    public void StartControlGame()
    {
        GLMenusStationsManager.Instance.OpenMenu(GLMenusStationsManager.AvailableStations.Control);
        glChecker.RestartControlGame();
    }

}
