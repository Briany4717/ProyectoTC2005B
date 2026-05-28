using UnityEngine;

/// <summary>
/// Detecta cuando el jugador alcanza el punto final del minijuego de control.
/// </summary>
public class GLFinalPoint : MonoBehaviour
{
    [SerializeField] StationData stationToComplete;

    /// <summary>
    /// Completa la estación actual al colisionar con el objeto verificador.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GLChecker"))
        {
            Debug.Log("Llegaste al final!!!");
            GLMenusStationsManager.Instance.CloseAllMenus();
            OrderManager.Instance.OnPlayerCompletedStation(stationToComplete);
        }
    }
}